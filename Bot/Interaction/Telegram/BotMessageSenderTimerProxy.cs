using Hedgey.Rx;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class BotMessageSenderTimerProxy : IMessageSender, IDisposable
{
  const int MAX_SENDS_PER_SECOND = 30;
  static readonly TimeSpan second = TimeSpan.FromSeconds(1);
  private readonly IMessageSender sender;
  BottleNeck<long> trafficController;
  public BotMessageSenderTimerProxy(IMessageSender sender)
  {
    this.sender = sender;
     trafficController = new BottleNeck<long>(Observable.Timer(second), MAX_SENDS_PER_SECOND);
  }

  private void OnException(Exception exception)
  {
    Console.WriteLine(exception);
  }

  public virtual void Send(ChatId chatId, string text, IReplyMarkup? inlineMarkup = default, bool silent = true)
  {
    _ = trafficController.Next().Subscribe(_ => sender.Send(chatId, text, inlineMarkup, silent), OnException);
  }

  public void Send(SendMessage message)
  {
    _ = trafficController.Next().Subscribe(_ => sender.Send(message), OnException);
  }

  public IObservable<Message> ObservableSend(SendMessage message)
  => trafficController.Next().SelectMany(sender.ObservableSend(message));

  public IObservable<Message> ObservableSend(MessageBuilder messageBuilder)
  => trafficController.Next().SelectMany(sender.ObservableSend(messageBuilder));

  public void Dispose()
  {
    trafficController.Dispose();
  }

  public record ResponseMessage(ChatId chatId, string text, IReplyMarkup? inlineMarkup = default, bool silent = true);
}