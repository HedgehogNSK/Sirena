using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot
{
  public abstract  class AbstractBotMessageSender : IMessageSender, IMessageForwarder, IMessageCopier
  {
    public abstract IObservable<MessageIdObject> Copy(CopyMessage message);

    public  abstract IObservable<MessageIdObject[]> Copy(CopyMessages messages);

    public  abstract IObservable<Message> Forward(ForwardMessage message);

    public  abstract IObservable<MessageIdObject[]> Forward(ForwardMessages messages);

    public  abstract IObservable<Message> ObservableSend(SendMessage message);

    public  abstract IObservable<Message> ObservableSend(MessageBuilder messageBuilder);

    public  abstract void Send(ChatId chatId, string text, IReplyMarkup? markup = null, bool silent = true);

    public  abstract void Send(SendMessage message);
  }
}