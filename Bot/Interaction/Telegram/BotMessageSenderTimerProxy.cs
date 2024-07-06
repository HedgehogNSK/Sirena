using Hedgey.Rx;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class BotMessageSenderTimerProxy : AbstractBotMessageSender, IMessageSender, IMessageForwarder, IMessageCopier, IDisposable
{
  const int MAX_SENDS_PER_SECOND = 30;
  static readonly TimeSpan second = TimeSpan.FromSeconds(1);
  private readonly IMessageSender sender;
  private readonly IMessageCopier messageCopier;
  private readonly IMessageForwarder messageForwarder;
  BottleNeck trafficController;
  public BotMessageSenderTimerProxy(AbstractBotMessageSender sender)
  {
    this.sender = sender;
    this.messageCopier = sender;
    this.messageForwarder = sender;
    var observableLimitReset = Observable.Timer(second).Select(_ => Unit.Default);
    trafficController = new BottleNeck(observableLimitReset, MAX_SENDS_PER_SECOND);
  }

  private void OnException(Exception exception)
  {
    Console.WriteLine(exception);
  }

  public override void Send(ChatId chatId, string text, IReplyMarkup? inlineMarkup = default, bool silent = true)
  {
    _ = trafficController.Next().Subscribe(_ => sender.Send(chatId, text, inlineMarkup, silent), OnException);
  }

  public override void Send(SendMessage message)
  {
    _ = trafficController.Next().Subscribe(_ => sender.Send(message), OnException);
  }

  public override IObservable<Message> ObservableSend(SendMessage message)
  => trafficController.Next().SelectMany(sender.ObservableSend(message));
  public override IObservable<Message> ObservableSend(IMessageBuilder messageBuilder)
  => trafficController.Next().SelectMany(sender.ObservableSend(messageBuilder));
  public override IObservable<MessageIdObject> Copy(CopyMessage message)
  => trafficController.Next().SelectMany(messageCopier.Copy(message));
  public override IObservable<MessageIdObject[]> Copy(CopyMessages messages)
  => trafficController.Next().SelectMany(messageCopier.Copy(messages));
  public override IObservable<Message> Forward(ForwardMessage message)
  => trafficController.Next().SelectMany(messageForwarder.Forward(message));
  public override IObservable<MessageIdObject[]> Forward(ForwardMessages messages)
  => trafficController.Next().SelectMany(messageForwarder.Forward(messages));

  public void Dispose()
  {
    trafficController.Dispose();
  }
}