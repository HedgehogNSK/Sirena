using Hedgey.Extensions;
using Hedgey.Structure.Factory;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.Setup;

namespace Hedgey.Sirena;

public class TelegramHelpFactory : IFactory<TelegramBot>
{
  private readonly string token;
  private readonly IObservable<Update> updateStream;

  public TelegramHelpFactory(IObservable<Update> updateHandler)
  {
    const string botToken = "SIRENA_TOKEN";
    token = OSTools.GetEnvironmentVar(botToken);
    this.updateStream = updateHandler;
  }
  TelegramBot IFactory<TelegramBot>.Create()
    => new TelegramBot.Builder(token).SetTracker(updateStream).Build();
}
