using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IMessageForwarder{
  IObservable<Message> Forward(ForwardMessage message);
  IObservable<MessageIdObject[]> Forward(ForwardMessages messages);
}