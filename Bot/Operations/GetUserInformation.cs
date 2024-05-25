using Hedgey.Extensions.Telegram;
using RxTelegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot.Operations;

public class GetUserInformation : IGetUserInformation
{
  private readonly TelegramBot bot;

  public GetUserInformation(TelegramBot bot)
  {
    this.bot = bot;
  }

  public IObservable<string> GetNickname(long uid)
  {
    return Observable.FromAsync(() => BotTools.GetDisplayName(bot, uid));
  }
}