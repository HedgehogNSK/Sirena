using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot
{
  public class BotMesssageSender : AbstractBotMessageSender, IMessageSender, IMessageForwarder, IMessageCopier
  {
    private readonly TelegramBot bot;

    public BotMesssageSender(TelegramBot bot)
    {
      this.bot = bot;
    }

    const string copyErrorMessage = "Exception on copy message [{0}|{1}] to chat: {2}";
    public override IObservable<MessageIdObject> Copy(CopyMessage message)
    {
      return Observable.Defer(() => Observable.FromAsync(() => bot.CopyMessage(message)))
      .Catch((Exception _exception)
        => throw new InvalidOperationException(string.Format(copyErrorMessage, message.FromChatId.Identifier, message.MessageId, message.ChatId.Identifier), _exception));
    }

    public override IObservable<MessageIdObject[]> Copy(CopyMessages messages)
    {
      return Observable.Defer(() => Observable.FromAsync(() => bot.CopyMessages(messages)))
        .Catch((Exception _ex) =>
        {
          var stringOfIds = string.Join(';', messages.MessageIds.Select(x => x.ToString()));
          throw new InvalidOperationException(string.Format(copyErrorMessage
          , messages.FromChatId.Identifier, stringOfIds, messages.ChatId.Identifier)
          , _ex);
        });
    }

    public override IObservable<bool> Edit(EditMessageMedia message) 
      => Observable.Defer(() => Observable.FromAsync(() => bot.EditMessageMedia(message)))
          .Catch((Exception _ex) =>
          {
            const string editErrorMessage = "Exception on edit message {0} in chat: {1}";
            throw new InvalidOperationException(string.Format(editErrorMessage
              , message.MessageId, message.ChatId.Identifier)
            , _ex);
          });

    public override IObservable<Message> Edit(EditMessageCaption message) 
      => Observable.Defer(() => Observable.FromAsync(() => bot.EditMessageCaption(message)))
          .Catch((Exception _ex) =>
          {
            const string editErrorMessage = "Exception on edit message caption {0} in chat: {1}";
            throw new InvalidOperationException(string.Format(editErrorMessage
              , message.MessageId, message.ChatId.Identifier)
            , _ex);
          });

    public override IObservable<Message> Edit(EditMessageReplyMarkup message) 
      => Observable.Defer(() => Observable.FromAsync(() => bot.EditMessageReplyMarkup(message)))
          .Catch((Exception _ex) =>
          {
            const string editErrorMessage = "Exception on edit message reply markup {0} in chat: {1}";
            throw new InvalidOperationException(string.Format(editErrorMessage
              , message.MessageId, message.ChatId.Identifier)
            , _ex);
          });

    public override IObservable<Message> Edit(EditMessageText message) 
      => Observable.Defer(() => Observable.FromAsync(() => bot.EditMessageText(message)))
          .Catch((Exception _ex) =>
          {
            const string editErrorMessage = "Exception on edit message {0} in chat: {1}";
            throw new InvalidOperationException(string.Format(editErrorMessage
              , message.MessageId, message.ChatId.Identifier)
            , _ex);
          });

    public override IObservable<Message> Edit(IEditMessageBuilder messageBuilder)
    {
      return Edit(messageBuilder.Build()).Catch((Exception _exception)
        => throw new InvalidOperationException($"Exception on edit message made by builder {messageBuilder.GetType().Name}", _exception));
    }

    const string forwardErrorMessage = "Exception on forward message [{0}|{1}] to chat: {2}";

    public override IObservable<Message> Forward(ForwardMessage message)
      => Observable.Defer(() => Observable.FromAsync(() => bot.ForwardMessage(message)))
          .Catch((Exception _ex)
            => throw new InvalidOperationException(string.Format(forwardErrorMessage, message.FromChatId.Identifier
            , message.MessageId, message.ChatId.Identifier), _ex));

    public override IObservable<MessageIdObject[]> Forward(ForwardMessages messages)
      => Observable.Defer(() => Observable.FromAsync(() => bot.ForwardMessages(messages)))
          .Catch((Exception _ex) =>
            {
              var stringOfIds = string.Join(';', messages.MessageIds.Select(x => x.ToString()));
              throw new InvalidOperationException(string.Format(forwardErrorMessage
              , messages.FromChatId.Identifier, stringOfIds, messages.ChatId.Identifier)
              , _ex);
            });

    public override IObservable<Message> ObservableSend(SendMessage message)
    {
      return Observable.FromAsync(() => bot.SendMessage(message))
      .Catch((Exception _exception)
        => throw new InvalidOperationException($"Exception on sending message to chat: {message.ChatId.Identifier}", _exception));
    }
    public override IObservable<Message> ObservableSend(ISendMessageBuilder messageBuilder)
    {
      var message = messageBuilder.Build();
      return ObservableSend(message)
        .Catch((Exception _exception)
          => throw new InvalidOperationException($"Exception on sending message made by builder {messageBuilder.GetType().Name}", _exception));
    }

    public override void Send(ChatId chatId, string text, IReplyMarkup? markup = null, bool silent = true)
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

    public async override void Send(SendMessage message)
    {
      try
      {
        _ = await bot.SendMessage(message);
      }
      catch (ApiException ex)
      {
        var sendException = new InvalidOperationException($"Exception on sending text to chat: {message.ChatId.Identifier} reason: {ex.StatusCode}\n{ex.Description}", ex);
        Console.WriteLine(sendException);
      }
    }
  }
}