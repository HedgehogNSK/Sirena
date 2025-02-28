using Hedgey.Rx;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class BotMessageSenderTimerProxy : AbstractBotMessageSender, IMessageSender
  , IMessageForwarder, IMessageCopier, IMessageEditor, IDisposable
{
  const int MAX_SENDS_PER_SECOND = 30;
  static readonly TimeSpan second = TimeSpan.FromSeconds(1);
  private readonly IMessageSender sender;
  private readonly IMessageCopier messageCopier;
  private readonly IMessageForwarder messageForwarder;
  private readonly IMessageEditor messageEditor;
  BottleNeck trafficController;
  public BotMessageSenderTimerProxy(AbstractBotMessageSender sender)
  {
    this.sender = sender;
    messageCopier = sender;
    messageForwarder = sender;
    messageEditor = sender;
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

  public IObservable<T> WrapByDispatcher<T>(IObservable<T> observable)
   => trafficController.Next().SelectMany(observable);

  public override IObservable<Message> ObservableSend(SendMessage message)
  => WrapByDispatcher(sender.ObservableSend(message));
  public override IObservable<Message> ObservableSend(ISendMessageBuilder messageBuilder)
  => WrapByDispatcher(sender.ObservableSend(messageBuilder));

  public override IObservable<MessageIdObject> Copy(CopyMessage message)
  => WrapByDispatcher(messageCopier.Copy(message));
  public override IObservable<MessageIdObject[]> Copy(CopyMessages messages)
  => WrapByDispatcher(messageCopier.Copy(messages));

  public override IObservable<bool> Edit(EditMessageMedia message)
  => WrapByDispatcher(messageEditor.Edit(message));
  public override IObservable<Message> Edit(EditMessageCaption message)
  => WrapByDispatcher(messageEditor.Edit(message));
  public override IObservable<Message> Edit(EditMessageReplyMarkup message)
  => WrapByDispatcher(messageEditor.Edit(message));
  public override IObservable<Message> Edit(EditMessageText message)
  => WrapByDispatcher(messageEditor.Edit(message));
  public override IObservable<Message> Edit(IEditMessageBuilder messageBuilder)
  => WrapByDispatcher(messageEditor.Edit(messageBuilder));

  public override IObservable<Message> Forward(ForwardMessage message)
  => WrapByDispatcher(messageForwarder.Forward(message));
  public override IObservable<MessageIdObject[]> Forward(ForwardMessages messages)
  => WrapByDispatcher(messageForwarder.Forward(messages));

  bool _disposed = false;

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void Dispose(bool explicitDisposing)
  {
    if (!_disposed)
    {
      if (explicitDisposing)
        DisposeManaged();

      DisposeUnmanaged();
      _disposed = true;
    }
  }
  /// <summary>
  /// Add external umanaged sources here
  /// such as COM-objects and P/Invoke
  /// </summary>
  protected virtual void DisposeUnmanaged() { }
  /// <summary>
  /// Add disposables here
  /// </summary>
  protected virtual void DisposeManaged()
  {
    trafficController?.Dispose();
  }

  ~BotMessageSenderTimerProxy()
  {
    Dispose(false);
  }
}