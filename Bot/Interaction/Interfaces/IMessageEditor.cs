using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IMessageEditor
{
  IObservable<bool> Edit(EditMessageMedia message);
  IObservable<Message> Edit(EditMessageCaption message);
  IObservable<Message> Edit(EditMessageReplyMarkup message);
  IObservable<Message> Edit(EditMessageText message);
  IObservable<Message> Edit(IEditMessageBuilder messageBuilder);
}

