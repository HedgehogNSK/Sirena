using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class CallSirenaStep : CommandStep
{
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly NullableContainer<Message> messageContainer;
  private readonly IMessageSender messageSender;
  private readonly IMessageCopier messageCopier;
  private readonly IUpdateSirenaOperation updateSirenaOperation;
  private readonly IFactory<IRequestContext, int, SirenRepresentation, IMessageBuilder> callReportMessageBuilderFactory;
  private readonly IFactory<long, IRequestContext, SirenRepresentation, IMessageBuilder> callServiceMessageBuilderFactory;

  /// <summary>
  ///
  /// </summary>
  /// <param name="sirenaContainer"></param>
  /// <param name="messageContainer"></param>
  /// <param name="messageSender"></param>
  /// <param name="messageCopier"></param>
  /// <param name="updateSirenaOperation"></param>
  /// <param name="callReportMessageBuilderFactory">Factory that creates message information to caller about notified users</param>
  /// <param name="callServiceMessageBuilderFactory">Factory that creates service message for subscribers</param>
  public CallSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer
  , NullableContainer<Message> messageContainer
  , IMessageSender messageSender
  , IMessageCopier messageCopier
  , IUpdateSirenaOperation updateSirenaOperation
  , IFactory<IRequestContext, int, SirenRepresentation, IMessageBuilder> callReportMessageBuilderFactory
  , IFactory<long, IRequestContext, SirenRepresentation, IMessageBuilder> callServiceMessageBuilderFactory)
  {
    this.sirenaContainer = sirenaContainer;
    this.messageSender = messageSender;
    this.updateSirenaOperation = updateSirenaOperation;
    this.callReportMessageBuilderFactory = callReportMessageBuilderFactory;
    this.callServiceMessageBuilderFactory = callServiceMessageBuilderFactory;
    this.messageContainer = messageContainer;
    this.messageCopier = messageCopier;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var sirena = sirenaContainer.Get();
    var chatId = context.GetTargetChatId();
    var info = context.GetCultureInfo();
    var uid = context.GetUser().Id;
    Stack<long> receiversStack = GetReceiversStack(sirena, uid);
    SirenRepresentation.CallInfo callInfo = new(uid, DateTimeOffset.Now);

    var observableNotification = updateSirenaOperation.UpdateLastCall(sirena.Sid, callInfo)
      .SelectMany(NotifySubscriber(context, receiversStack, sirena)).Publish().RefCount();

    return observableNotification
      .SelectMany(copyMessages => BroadcastNotification(copyMessages, receiversStack))
      .Count()
      .Zip(observableNotification, (_forwards, _copy) => _forwards + 1)
      .Select(CreateReport);

    Report CreateReport(int notifications)
    {
      if (notifications != 0)
        return new(Result.Success, callReportMessageBuilderFactory.Create(context, notifications, sirena));
      else
        return new(Result.Canceled);
    }
  }

  private IObservable<MessageIdObject[]> BroadcastNotification(CopyMessages copyMessages, Stack<long> receiversStack)
  {
    return receiversStack.Select(ReplaceTargetId).Concat();

    IObservable<MessageIdObject[]> ReplaceTargetId(long uid)
      => Observable.Defer(() =>
          {
            copyMessages.ChatId = uid;
            return messageCopier.Copy(copyMessages);
          })
          .Catch((Exception _ex) =>
          {
            var messageIds = copyMessages.MessageIds.Select(_id => _id.ToString());
            var idsString = string.Join(';', messageIds);
            var message = $"Resend copy messages exception\nFrom: {copyMessages.FromChatId}; To: {copyMessages.ChatId}; Messages:[{idsString}]";
            var wrapException = new Exception(message, _ex);
            Console.WriteLine(wrapException);
            return Observable.Empty<MessageIdObject[]>();
          });
  }

  ///<summary>
  ///We have to notify users when Sirena called
  ///First user will get copy message from caller and new system message
  ///Other users will get copy messages from first user
  ///If first user can't be notified (e.g. user blocked the bot), bot takes next user
  ///from stack and tryies to notified him
  /// </summary>
  /// <param name="receivers">Stack of users to notify</param>
  /// <param name="sirena">called Sirena</param>
  /// <returns></returns>
  private IObservable<CopyMessages> NotifySubscriber(IRequestContext context, Stack<long> receivers, SirenRepresentation sirena)
  {
    return Observable.Defer(() =>
    {
      if (!receivers.TryPop(out var uid))
        return Observable.Empty<CopyParams>();

      IMessageBuilder sirenaMessage = callServiceMessageBuilderFactory.Create(uid,context, sirena);

      var observableSendMessage = messageSender.ObservableSend(sirenaMessage)
            .Select(x => (long)x.MessageId);

      if (messageContainer.IsSet())
      {
        Message message = messageContainer.Get();
        CopyMessage copyMessage = new CopyMessage
        {
          FromChatId = message.From.Id,
          MessageId = message.MessageId,
          ProtectContent = true,
          ChatId = uid
        };

        var sendCopyToFirstSubscriber = messageCopier.Copy(copyMessage)
        .Select(_id => (long)_id.MessageId);

        observableSendMessage = observableSendMessage.Concat(sendCopyToFirstSubscriber);
      }
      return observableSendMessage.ToArray().Select(_messagesIds => new CopyParams(_messagesIds, uid));
    }).Retry().Select(CreateCopyMessages);
  }
  record struct CopyParams(long[] messageIDs, long fromUID);

  private static CopyMessages CreateCopyMessages(CopyParams copyParams)
    => CreateCopyMessages(copyParams.messageIDs, copyParams.fromUID);
  private static CopyMessages CreateCopyMessages(long[] messageIDs, long current)
    => new CopyMessages()
    {
      FromChatId = current,
      MessageIds = messageIDs.ToList(),
      DisableNotification = false,
      ProtectContent = true,
    };

  private static long[] GetReceiversArray(SirenRepresentation sirena, long uid)
  {
    long[] target = sirena.Listener;
    if (uid != sirena.OwnerId)
    {
      int index = Array.IndexOf(target, uid);
      target[index] = sirena.OwnerId;
    }

    return target;
  }
  private static Stack<long> GetReceiversStack(SirenRepresentation sirena, long uid)
    => new(GetReceiversArray(sirena, uid).Reverse());
  public record CopySettings(CopyMessages copyMessages, long[] recieverIds);

  public class Factory(IMessageSender messageSender, IMessageCopier messageCopier, IUpdateSirenaOperation updateSirenaOperation, IFactory<IRequestContext, int, SirenRepresentation, IMessageBuilder> callReportMessageBuilderFactory, IFactory<long,IRequestContext, SirenRepresentation, IMessageBuilder> callServiceMessageBuilderFactory) : IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep>
  {
    public CallSirenaStep Create(NullableContainer<SirenRepresentation> sirenaContainer, NullableContainer<Message> messageContainer) => new CallSirenaStep(sirenaContainer, messageContainer, messageSender, messageCopier, updateSirenaOperation, callReportMessageBuilderFactory, callServiceMessageBuilderFactory);
  }
}