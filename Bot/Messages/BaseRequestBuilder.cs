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
public abstract class BaseRequestBuilder<T> : IBaseRequestBuilder
where T : BaseRequestBuilder<T>
{
  protected long ChatID { get; private set; }

  public T Set(long chatId)
  {
    this.ChatID = chatId;
    return (T)this;
  }

  public abstract BaseRequest Build();
}