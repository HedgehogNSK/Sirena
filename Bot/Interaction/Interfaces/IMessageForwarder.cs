using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IMessageForwarder{
  IObservable<ForwardingReport[]> Forward(ForwardMessage message, params long[] targetArray);
  IObservable<bool> Forward(ForwardMessages message);
}

public record ForwardingReport(ChatId ChatId, bool Success, Exception Exception = null);