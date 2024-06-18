using RxTelegram.Bot.Interface.BaseTypes.Enums;
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
  public void ChangeTarget(long chatId)=> this.chatId = chatId;
  public abstract SendMessage Build();
  protected SendMessage CreateDefault(string message, IReplyMarkup? replyMarkup = null) 
    => new SendMessage()
        {
          ChatId = chatId,
          DisableNotification = true,
          ProtectContent = false,
          Text = message,
          ReplyMarkup = replyMarkup,
          ParseMode = ParseMode.Markdown
        };
}
