using Hedgey.Sirena.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Telegram.Messages;

public abstract class SendMessageBuilder<T> : BaseRequestBuilder<T>, ISendMessageBuilder
where T : SendMessageBuilder<T>
{
  public override abstract SendMessage Build();
  protected SendMessage CreateDefault(string message, IReplyMarkup? replyMarkup = null)
    => MarkupShortcuts.CreateDefaultMessage(ChatID, message, replyMarkup);
}