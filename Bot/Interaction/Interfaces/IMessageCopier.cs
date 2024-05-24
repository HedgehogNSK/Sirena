using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IMessageCopier{
  IObservable<MessageIdObject> Copy(CopyMessage message);
  IObservable<MessageIdObject[]> Copy(CopyMessages messages);
}
