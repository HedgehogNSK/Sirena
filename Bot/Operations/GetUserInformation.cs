
using System.Reactive.Threading.Tasks;
using Hedgey.Extensions.Telegram;
using RxTelegram.Bot;

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
    return BotTools.GetUsername(bot, uid).ToObservable();
  }
}