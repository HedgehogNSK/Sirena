using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hedgey.Sirena.Bot;

public class BotMessageSenderTimerProxy : IMessageSender, IDisposable
{
  const int MAX_SENDS_PER_SECOND = 30;
  static readonly TimeSpan second = TimeSpan.FromSeconds(1);
  private readonly IMessageSender sender;
  int count = 0;
  protected ConcurrentQueue<ResponseMessage> waitingMessage = new ConcurrentQueue<ResponseMessage>();
  Subject<Unit> timerStarter = new Subject<Unit>();
  IDisposable stream;
  public BotMessageSenderTimerProxy(IMessageSender sender)
  {
    IObservable<ResponseMessage> observableMessages = Enumerable.Range(0, MAX_SENDS_PER_SECOND)
        .ToObservable()
        .TakeWhile(_ => count < MAX_SENDS_PER_SECOND && !waitingMessage.IsEmpty)
        .Select(_ => waitingMessage.TryDequeue(out var message) ? message : new ResponseMessage(0, string.Empty, false));

    stream = timerStarter.SelectMany(Observable.Timer(second))
        .Do(_ => count = 0)
        .SelectMany(observableMessages)
        .Subscribe(SendImmidiate);
    this.sender = sender;
  }
  public void SendImmidiate(ResponseMessage param)
  {
    if (Interlocked.Increment(ref count) == 1)
      timerStarter.OnNext(Unit.Default);

    sender.Send(param.chatId, param.text, param.silent);
  }
  public virtual void Send(long chatId, string text, bool silent = true)
  {
    ResponseMessage response = new(chatId, text, silent);
    if (count < MAX_SENDS_PER_SECOND && waitingMessage.IsEmpty)
    {
      SendImmidiate(response);
    }
    else
    {
      waitingMessage.Enqueue(response);
    }
  }

  public void Dispose()
  {
    timerStarter?.Dispose();
    stream?.Dispose();
  }

  public record ResponseMessage(long chatId, string text, bool silent);
}