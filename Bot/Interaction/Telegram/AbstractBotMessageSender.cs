using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot
{
  public abstract class AbstractBotMessageSender : IMessageSender
    , IMessageForwarder, IMessageCopier, IMessageEditor
  {
    public abstract IObservable<MessageIdObject> Copy(CopyMessage message);
    public abstract IObservable<MessageIdObject[]> Copy(CopyMessages messages);

    public abstract IObservable<bool> Edit(EditMessageMedia message);
    public abstract IObservable<Message> Edit(EditMessageCaption message);
    public abstract IObservable<Message> Edit(EditMessageReplyMarkup message);
    public abstract IObservable<Message> Edit(IEditMessageReplyMarkupBuilder message);
    public abstract IObservable<Message> Edit(EditMessageText message);
    public abstract IObservable<Message> Edit(IEditMessageBuilder messageBuilder);

    public abstract IObservable<Message> Forward(ForwardMessage message);
    public abstract IObservable<MessageIdObject[]> Forward(ForwardMessages messages);

    public abstract IObservable<Message> ObservableSend(SendMessage message);
    public abstract IObservable<Message> ObservableSend(ISendMessageBuilder messageBuilder);

    public abstract void Send(ChatId chatId, string text, IReplyMarkup? markup = null, bool silent = true);
    public abstract void Send(SendMessage message);
  }
}