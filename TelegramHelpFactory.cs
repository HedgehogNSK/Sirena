using Hedgey.Extensions;
using Hedgey.Structure.Factory;
using RxTelegram.Bot;
using Telegram.Bot;

namespace Hedgey.Sirena;

public class TelegramHelpFactory : IFactory< TelegramBot>,IFactory<TelegramBotClient>
{
  private readonly string token;

  public TelegramHelpFactory(){
    const string tokenKey = "SIRENA_TOKEN";
    token = OSTools.GetEnvironmentVar(tokenKey);
    if(string.IsNullOrEmpty(token))
        throw new ArgumentException("TelegramBot can't be created. Reason: wrong token.");
}
  TelegramBot IFactory<TelegramBot>.Create() => new TelegramBot(token);
  TelegramBotClient IFactory<TelegramBotClient>.Create() => new TelegramBotClient(token);
}