using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IMessageSender{
  void Send(ChatId chatId, string text,IReplyMarkup? markup = default, bool silent = true);
  void Send(SendMessage message);
  IObservable<Message> ObservableSend(SendMessage message);
  IObservable<Message> ObservableSend(MessageBuilder messageBuilder);
}
