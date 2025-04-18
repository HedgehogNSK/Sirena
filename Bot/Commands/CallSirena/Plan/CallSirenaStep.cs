using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class CallSirenaStep : CommandStep
{
  private readonly NullableContainer<SirenaData> sirenaContainer;
  private readonly NullableContainer<Message> messageContainer;
  private readonly IMessageSender messageSender;
  private readonly IMessageCopier messageCopier;
  private readonly ISirenaActivationOperation activationOperation;
  private readonly IFactory<IRequestContext, int, SirenaData, ISendMessageBuilder> callReportMessageBuilderFactory;
  private readonly IFactory<IRequestContext, ServiceMessageData, ISendMessageBuilder> callServiceMessageBuilderFactory;

  /// <summary>
  ///
  /// </summary>
  /// <param name="sirenaContainer"></param>
  /// <param name="messageContainer"></param>
  /// <param name="messageSender"></param>
  /// <param name="messageCopier"></param>
  /// <param name="updateSirenaOperation"></param>
  /// <param name="activationOperation"></param>
  /// <param name="callReportMessageBuilderFactory">Factory that creates message information to caller about notified users</param>
  /// <param name="callServiceMessageBuilderFactory">Factory that creates service message for subscribers</param>
  public CallSirenaStep(NullableContainer<SirenaData> sirenaContainer
  , NullableContainer<Message> messageContainer
  , IMessageSender messageSender
  , IMessageCopier messageCopier
  , ISirenaActivationOperation activationOperation
  , IFactory<IRequestContext, int, SirenaData, ISendMessageBuilder> callReportMessageBuilderFactory
  , IFactory<IRequestContext, ServiceMessageData, ISendMessageBuilder> callServiceMessageBuilderFactory)
  {
    this.sirenaContainer = sirenaContainer;
    this.messageSender = messageSender;
    this.activationOperation = activationOperation;
    this.callReportMessageBuilderFactory = callReportMessageBuilderFactory;
    this.callServiceMessageBuilderFactory = callServiceMessageBuilderFactory;
    this.messageContainer = messageContainer;
    this.messageCopier = messageCopier;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var sirena = sirenaContainer.Get();
    var uid = context.GetUser().Id;
    var cid = context.GetChat().Id;
    Queue<long> receiversQueue = GetReceiversQueue(sirena, uid, cid);

    var observableCallInfo = activationOperation.LogInfo(sirena.SID, uid)
      .Publish()
      .RefCount(2);

    var observableNotification = observableCallInfo
      .SelectMany(_callInfo => NotifyFirstSubscriber(context, sirena, receiversQueue, _callInfo))
      .SelectMany(_pair => BroadcastNotification(_pair.CopyMessage, receiversQueue)
        .Do(_receiverIds => _pair.CurrentReceivers.AddRange(_receiverIds))
        .Select(_ => _pair.CurrentReceivers)
      )
      .Publish()
      .RefCount(2);

    _ = observableCallInfo.Zip(observableNotification)
      .SelectMany((_pair) => activationOperation.SetReceivers(_pair.First, _pair.Second))
      .Subscribe();

    return observableNotification.Select(_receivers => CreateReport(_receivers.Count));


    Report CreateReport(int notifications)
    {
      if (notifications != 0)
        return new(Result.Success, callReportMessageBuilderFactory.Create(context, notifications, sirena));
      else
        return new(Result.Canceled);
    }
  }

  private IObservable<long[]> BroadcastNotification(CopyMessages copyMessages
    , Queue<long> receiversStack)
  {
    return receiversStack.Select(ReplaceTargetId).Concat().ToArray();

    IObservable<long> ReplaceTargetId(long uid)
      => Observable.Defer(() =>
          {
            copyMessages.ChatId = uid;
            return messageCopier.Copy(copyMessages);
          })
          .Catch((Exception _ex) =>
          {
            var messageIds = copyMessages.MessageIds.Select(_id => _id.ToString());
            var idsString = string.Join(';', messageIds);
            var message = $"Couldn't send copy messages\nFrom: {copyMessages.FromChatId}; To: {copyMessages.ChatId}; Messages:[{idsString}]";
            Console.WriteLine(message);
            ExceptionHandler.OnError(_ex);
            return Observable.Empty<MessageIdObject[]>();
          })
          .Select(_ => uid);
  }

  ///<summary>
  /// We need to notify users when Sirena is called.
  /// The first user will receive a copy of the message from the caller and a new system message.
  /// Other users will receive copies of the messages from the first user.
  /// If the first user can't be notified (e.g., the user blocked the bot),
  /// the bot takes the next user from the stack and tries to notify them.
  /// </summary>
  /// <param name="receivers">Stack of users to notify</param>
  /// <param name="sirena">called Sirena</param>
  /// <returns></returns>
  private IObservable<(CopyMessages CopyMessage, List<long> CurrentReceivers)> NotifyFirstSubscriber(IRequestContext context
    , SirenaData sirena
    , Queue<long> receivers
    , SirenaActivation callInfo)
  {
    return Observable.Defer(() =>
    {
      if (!receivers.TryDequeue(out var uid))
        return Observable.Empty<CopyParams>();
      ServiceMessageData data = new(uid, sirena, callInfo);
      ISendMessageBuilder sirenaMessage = callServiceMessageBuilderFactory.Create(context, data);

      var observableSendMessage = messageSender.ObservableSend(sirenaMessage)
        .Select(x => (long)x.MessageId);

      if (messageContainer.IsSet)
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
      return observableSendMessage.ToArray()
        .Select(_messagesIds => new CopyParams(_messagesIds.ToList(), uid));
    }).Retry()
    .Select(x => (CreateCopyMessages(x), new List<long>() { x.fromUID }));
  }
  record struct CopyParams(List<long> messageIDs, long fromUID);

  private static CopyMessages CreateCopyMessages(CopyParams copyParams)
    => new CopyMessages()
    {
      FromChatId = copyParams.fromUID,
      MessageIds = copyParams.messageIDs,
      DisableNotification = false,
      ProtectContent = true,
    };

  private unsafe static IEnumerable<long> GetReceiversArrayViaPtr(long[] listeners, long ownerId, long userId, long chatId)
  {
    int length = listeners.Length;
    var array = new long[length];
    fixed (long* sourceBegin = listeners)
    fixed (long* resultBegin = array)
    {
      long* result = resultBegin;
      if (userId != ownerId)
      {
        *result = ownerId;
        ++result;
      }
      for (long* source = sourceBegin; source != sourceBegin + length; ++source)
      {
        if (*source == userId)
          continue;
        else if (*source == chatId)
          continue;

        *result = *source;
        ++result;
      }
      length -= (int)(result - resultBegin);
      return array.SkipLast(length);
    }
  }
  private static IEnumerable<long> GetReceiversArrayViaArray(long[] listeners, long ownerId, long userId, long chatId)
  {
    long[] result;
    if (chatId != userId)
    {
      result = new long[listeners.Length];
      int rd = 0;
      if (userId != ownerId)
        result[rd++] = ownerId;

      for (int id = 0; id != listeners.Length; ++id)
      {
        var value = listeners[id];
        if (value == chatId)
          continue;
        else if (value == userId)
          continue;

        result[rd] = value;
        ++rd;
      }
      Array.Resize(ref result, rd);
    }
    else
    {
      result = (long[])listeners.Clone();

      if (userId != ownerId)
      {
        int id = Array.IndexOf(result, userId);
        result[id] = ownerId;
      }
    }

    return result;
  }
  private static IEnumerable<long> GetReceiversArrayViaList(long[] listeners, long ownerId, long userId, long chatId)
  {
    List<long> receivers = new(listeners);

    if (userId != chatId)
      receivers.Remove(chatId);

    if (userId != ownerId)
    {
      receivers.Remove(userId);
      receivers.Add(ownerId);
    }

    return receivers;
  }
  /// <summary>
  /// Evaluates target chats to send message from caller
  /// </summary>
  /// <param name="sirena"></param>
  /// <param name="userId">Caller ID</param>
  /// <param name="chatId">The ID of the chat where Sirena was called from.</param>
  /// <returns></returns>
  unsafe public static IEnumerable<long> GetReceiversArray(SirenaData sirena, long userId, long chatId)
    => GetReceiversArrayViaList(sirena.Listener, sirena.OwnerId, userId, chatId);
  private static Queue<long> GetReceiversQueue(SirenaData sirena, long uid, long cid)
    => new(GetReceiversArray(sirena, uid, cid));
  public record CopySettings(CopyMessages copyMessages, long[] recieverIds);
  public class Factory(IMessageSender messageSender, IMessageCopier messageCopier
    , ISirenaActivationOperation activationInformation
    , IFactory<IRequestContext, int, SirenaData, ISendMessageBuilder> callReportMessageBuilderFactory
    , IFactory<IRequestContext, ServiceMessageData, ISendMessageBuilder> callServiceMessageBuilderFactory)
    : IFactory<NullableContainer<SirenaData>, NullableContainer<Message>, CallSirenaStep>
  {
    public CallSirenaStep Create(NullableContainer<SirenaData> sirenaContainer, NullableContainer<Message> messageContainer) => new CallSirenaStep(sirenaContainer, messageContainer, messageSender, messageCopier, activationInformation, callReportMessageBuilderFactory, callServiceMessageBuilderFactory);
  }
}
