using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot
{
  public class BotMesssageSender : IMessageSender, IMessageForwarder, IMessageCopier
  {
    private readonly TelegramBot bot;

    public BotMesssageSender(TelegramBot bot)
    {
      this.bot = bot;
    }

    const string copyErrorMessage = "Exception on copy message [{0}|{1}] to chat: {2}";
    public IObservable<MessageIdObject> Copy(CopyMessage message)
    {
      return Observable.Defer(() => Observable.FromAsync(() => bot.CopyMessage(message)))
      .Catch((Exception _exception)
        => throw new Exception(string.Format(copyErrorMessage, message.FromChatId.Identifier, message.MessageId, message.ChatId.Identifier), _exception));
    }

    public IObservable<MessageIdObject> Copy(CopyMessage message, params long[] targetArray)
    {
      return targetArray.Select(CreateCopyForCertainChat).Concat();

      IObservable<MessageIdObject> CreateCopyForCertainChat(long chatId, int index)
      {
        return Observable.Defer(() =>
            {
              message.ChatId = chatId;
              return Observable.Return(message);
            })
            .SelectMany(Copy);
      }
    }

    public IObservable<MessageIdObject[]> Copy(CopyMessages messages)
    {
      return Observable.Defer(() => Observable.FromAsync(() => bot.CopyMessages(messages)))
        .Catch((Exception _ex) =>
        {
          var stringOfIds = string.Join(';', messages.MessageIds.Select(x => x.ToString()));
          throw new Exception(string.Format(copyErrorMessage
          , messages.FromChatId.Identifier, stringOfIds, messages.ChatId.Identifier)
          , _ex);
        });
    }

    public IObservable<MessageIdObject[]> Copy(CopyMessages copy, params long[] targetArray)
    {
      return targetArray.Select(CreateCopyForCertainChat).Concat();

      IObservable<MessageIdObject[]> CreateCopyForCertainChat(long chatId, int index)
      {
        return Observable.Defer(() =>
            {
              copy.ChatId = chatId;
              return Observable.Return(copy);
            })
            .SelectMany(Copy);
      }
    }
    const string forwardErrorMessage = "Exception on forward message [{0}|{1}] to chat: {2}";
    public IObservable<Message> Forward(ForwardMessage message, params long[] targetArray)
    {
      return targetArray.Select(CreateForwardToUser).Concat();

      IObservable<Message> CreateForwardToUser(long chatId, int index)
      {
        return Observable.Defer(() =>
            {
              message.ChatId = chatId;
              return Observable.Return(message);
            })
            .SelectMany(Forward);
      }
    }

    public IObservable<Message> Forward(ForwardMessage message)
      => Observable.Defer(() => Observable.FromAsync(() => bot.ForwardMessage(message)))
          .Catch((Exception _ex)
            => throw new Exception(string.Format(forwardErrorMessage, message.FromChatId.Identifier
            , message.MessageId, message.ChatId.Identifier), _ex));

    public IObservable<MessageIdObject[]> Forward(ForwardMessages messages, params long[] targetArray)
    {
      return targetArray.Select(CreateForwardToUser).Concat();

      IObservable<MessageIdObject[]> CreateForwardToUser(long chatId, int index)
      {
        return Observable.Defer(() =>
            {
              messages.ChatId = chatId;
              return Observable.Return(messages);
            })
            .SelectMany(Forward);
      }
    }
    public IObservable<MessageIdObject[]> Forward(ForwardMessages messages)
      => Observable.Defer(() => Observable.FromAsync(() => bot.ForwardMessages(messages)))
          .Catch((Exception _ex) =>
            {
              var stringOfIds = string.Join(';', messages.MessageIds.Select(x => x.ToString()));
              throw new Exception(string.Format(forwardErrorMessage
              , messages.FromChatId.Identifier, stringOfIds, messages.ChatId.Identifier)
              , _ex);
            });

    public IObservable<Message> ObservableSend(SendMessage message)
    {

      return Observable.FromAsync(() => bot.SendMessage(message))
      .Catch((Exception _exception)
        => throw new Exception($"Exception on sending message to chat: {message.ChatId.Identifier}", _exception));
    }
    public IObservable<Message> ObservableSend(MessageBuilder messageBuilder)
    {
      return ObservableSend(messageBuilder.Build());
    }

    public void Send(ChatId chatId, string text, IReplyMarkup? markup, bool silent)
    {

      var sendMessage = new SendMessage
      {
        ChatId = chatId,
        Text = text,
        ReplyMarkup = markup,
        ProtectContent = false,
        DisableNotification = silent,
        ParseMode = ParseMode.Markdown,
      };
      Send(sendMessage);
    }

    public async void Send(SendMessage sendMessage)
    {
      try
      {
        Message message = await bot.SendMessage(sendMessage);
        var result = message;
      }
      catch (ApiException ex)
      {
        var sendException = new Exception($"Exception on sending text to chat: {sendMessage.ChatId.Identifier} reason: {ex.StatusCode}\n{ex.Description}", ex);
        Console.WriteLine(sendException);
      }
    }
  }
}