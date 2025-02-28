using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base;

namespace Hedgey.Sirena.Bot;

public abstract class BaseRequestBuilder : IBaseRequestBuilder
{
  protected ChatId ChatID { get; private set; }
  public BaseRequestBuilder(long chatId) => this.ChatID = chatId;
  public void SetTargetChat(long chatId)
  {
    this.ChatID = chatId;
  }

  public abstract BaseRequest Build();
}