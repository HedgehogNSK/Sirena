using Hedgey.Rx;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class BotMessageSenderTimerProxy : IMessageSender, IMessageForwarder, IMessageCopier, IDisposable
{
  const int MAX_SENDS_PER_SECOND = 30;
  static readonly TimeSpan second = TimeSpan.FromSeconds(1);
  private readonly IMessageSender sender;
  private readonly IMessageCopier messageCopier;
  private readonly IMessageForwarder messageForwarder;
  BottleNeck trafficController;
  public BotMessageSenderTimerProxy(IMessageSender sender, IMessageCopier messageCopier, IMessageForwarder messageForwarder)
  {
    this.sender = sender;
    this.messageCopier = messageCopier;
    this.messageForwarder = messageForwarder;
    var observableLimitReset = Observable.Timer(second).Select(_ => Unit.Default);
    trafficController = new BottleNeck(observableLimitReset, MAX_SENDS_PER_SECOND);
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
  public IObservable<MessageIdObject> Copy(CopyMessage message)
  => trafficController.Next().SelectMany(messageCopier.Copy(message));
  public IObservable<MessageIdObject[]> Copy(CopyMessages messages)
  => trafficController.Next().SelectMany(messageCopier.Copy(messages));
  public IObservable<Message> Forward(ForwardMessage message)
  => trafficController.Next().SelectMany(messageForwarder.Forward(message));
  public IObservable<MessageIdObject[]> Forward(ForwardMessages messages)
  => trafficController.Next().SelectMany(messageForwarder.Forward(messages));

  public void Dispose()
  {
    trafficController.Dispose();
  }
}