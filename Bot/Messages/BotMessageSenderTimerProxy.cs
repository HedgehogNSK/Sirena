using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

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
        .Select(_ => waitingMessage.TryDequeue(out var message) ? message : new ResponseMessage(0, string.Empty));

    stream = timerStarter.SelectMany(Observable.Timer(second))
        .Do(_ => count = 0)
        .SelectMany(observableMessages)
        .Subscribe(SendImmidiate,OnException);
    this.sender = sender;
  }

  private void OnException(Exception exception)
  {
     Console.WriteLine(exception);
  }

  public void SendImmidiate(ResponseMessage param)
  {
    if (Interlocked.Increment(ref count) == 1)
      timerStarter.OnNext(Unit.Default);
    try
    {
      sender.Send(param.chatId, param.text,param.inlineMarkup, param.silent);
    }
    catch (Exception innerException)
    {

      const string exMessage = "Exception on sending to chat {0} message:\n {1}";
      var ex = new Exception(string.Format(exMessage, param.chatId, param.text), innerException);
      Console.WriteLine(ex);
    }
  }
  public virtual void Send(ChatId chatId, string text, IReplyMarkup? inlineMarkup = default, bool silent = true)
  {
    ResponseMessage response = new(chatId, text, inlineMarkup, silent);
    if (count < MAX_SENDS_PER_SECOND && waitingMessage.IsEmpty)
    {
      SendImmidiate(response);
    }
    else
    {
      waitingMessage.Enqueue(response);
    }
  }

  public void Send(SendMessage message)
  {
    ResponseMessage response = new(message.ChatId, message.Text, message.ReplyMarkup, message.DisableNotification??true);
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

  public record ResponseMessage(ChatId chatId, string text,IReplyMarkup? inlineMarkup = default, bool silent=true);
}