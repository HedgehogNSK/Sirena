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
}
