using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public abstract class MessageBuilder
{
  protected long chatId;
  public MessageBuilder(long chatId)
  {
    this.chatId = chatId;
  }
  public abstract SendMessage Build();

  protected SendMessage CreateDefault(string message, IReplyMarkup replyMarkup = null)
  {
    return new SendMessage()
    {
      ChatId = chatId,
      DisableNotification = true,
      ProtectContent = true,
      Text = message,
      ReplyMarkup = replyMarkup,
    };
  }
}
