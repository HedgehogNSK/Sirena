using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IMessageEditor
{
  IObservable<bool> Edit(EditMessageMedia editMessageMedia);
  IObservable<Message> Edit(EditMessageCaption editMessageCaption);
  IObservable<Message> Edit(EditMessageReplyMarkup editReplyMarkup);
  IObservable<Message> Edit(IEditMessageReplyMarkupBuilder editReplyMarkupBuilder);
  IObservable<Message> Edit(EditMessageText editMessageText);
  IObservable<Message> Edit(IEditMessageBuilder messageBuilder);
}

