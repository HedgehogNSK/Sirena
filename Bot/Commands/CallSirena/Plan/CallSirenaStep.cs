using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;
using System.Runtime.InteropServices;

namespace Hedgey.Sirena.Bot;
public class CallSirenaStep : CommandStep
{
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly NullableContainer<Message> messageContainer;
  private readonly IMessageSender messageSender;
  private readonly IMessageCopier messageCopier;
  private readonly IUpdateSirenaOperation updateSirenaOperation;
  public CallSirenaStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer
  , NullableContainer<Message> messageContainer
  , IMessageSender messageSender
  , IMessageCopier messageCopier
  , IUpdateSirenaOperation updateSirenaOperation
)
   : base(contextContainer)
  {
    this.sirenaContainer = sirenaContainer;
    this.messageSender = messageSender;
    this.updateSirenaOperation = updateSirenaOperation;
    this.messageContainer = messageContainer;
    this.messageCopier = messageCopier;
  }

  public override IObservable<Report> Make()
  {
    var sirena = sirenaContainer.Get();
    var chatId = Context.GetTargetChatId();
    var info = Context.GetCultureInfo();
    var uid = Context.GetUser().Id;
    Stack<long> receiversStack = GetReceiversStack(sirena, uid);
    SirenRepresentation.CallInfo callInfo = new(uid, DateTimeOffset.Now);

    var observableNotification = updateSirenaOperation.UpdateLastCall(sirena.Id, callInfo)
      .SelectMany(NotifySubscriber(receiversStack, sirena)).Publish().RefCount();

    return observableNotification
      .SelectMany(copyMessages => BroadcastNotification(copyMessages, receiversStack))
      .Count()
      .Zip(observableNotification, (_forwards, _copy) => _forwards + 1)
      .Select(CreateReport);

    Report CreateReport(int notifications)
    {
      if (notifications != 0)
        return new(Result.Success, new SirenaCallReportMessageBuilder(chatId,info, Program.LocalizationProvider, notifications, sirena));
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
          .Catch((Exception _ex)=> {
            var messageIds = copyMessages.MessageIds.Select(_id => _id.ToString());
            var idsString = string.Join(';',messageIds);
            var message =$"Resend copy messages exception\nFrom: {copyMessages.FromChatId}; To: {copyMessages.ChatId}; Messages:[{idsString}]";
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
  private IObservable<CopyMessages> NotifySubscriber(Stack<long> receivers, SirenRepresentation sirena)
  {
    CopyMessage? copyMessage = null;
    if (messageContainer.Content != null)
    {
      Message message = messageContainer.Get();
      copyMessage = new CopyMessage()
      {
        FromChatId = message.From.Id,
        MessageId = message.MessageId,
        ProtectContent = true,
      };
    }
    MessageBuilder? sirenaMessage = null;
    var observableNotification = Observable.Defer(() =>
    {
      if (!receivers.TryPop(out var uid))
        return Observable.Empty<CopyParams>();

      if (sirenaMessage == null)
      {
        var info = Context.GetCultureInfo();
        sirenaMessage = new SirenaCallServiceMessageBuilder(uid, info, Program.LocalizationProvider, Context.GetUser(), sirena);
      }
      else
        sirenaMessage.ChangeTarget(uid);

      var observableSendMessage = messageSender.ObservableSend(sirenaMessage)
            .Select(x => (long)x.MessageId);

      if (copyMessage != null)
      {
        copyMessage.ChatId = uid;
        var sendCopyToFirstSubscriber = messageCopier.Copy(copyMessage)
        .Select(_id => (long)_id.MessageId);

        observableSendMessage = observableSendMessage.Concat(sendCopyToFirstSubscriber);
      }
      return observableSendMessage.ToArray().Select(_messagesIds => new CopyParams(_messagesIds, uid));
    });

    return observableNotification.Retry().Select(CreateCopyMessages);
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

  static T[] ArrayPartiailCopy<T>(T[] array, int startIndex)
  {
    int size = Marshal.SizeOf(typeof(T));
    int count = array.Length - startIndex;
    T[] actualReceivers = new T[count];
    Buffer.BlockCopy(array, startIndex * size, actualReceivers, 0, count * size);
    return actualReceivers;
  }
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
}