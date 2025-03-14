using Hedgey.Extensions.NetCoreServer;
using NetCoreServer;
using RxTelegram.Bot.Api;
using RxTelegram.Bot.Interface.Setup;

namespace Hedgey.Sirena.HTTP
{
  static public class Extension
  {
    static public Update ParseUpdate(HttpRequest request)
      => HttpExtension.Parse<Update>(request, BaseTelegramBot.JsonSerializerSettings);
  }
}