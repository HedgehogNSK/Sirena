using NetCoreServer;
using Newtonsoft.Json;
using RxTelegram.Bot.Api;
using RxTelegram.Bot.Interface.Setup;

namespace Hedgey.Extensions.NetCoreServer;

static public class HttpExtension
{
  static public T Parse<T>(HttpRequest request)
  {
    var serializer = JsonSerializer.Create(BaseTelegramBot.JsonSerializerSettings);
    var stringReader = new StringReader(request.Body);
    var jsonReader = new JsonTextReader(stringReader);
    T update = serializer.Deserialize<T>(jsonReader)
     ?? throw new ArgumentException($"HTTP request body is not {typeof(Update).Name}\n Request body: {request.Body}");
    return update;
  }
}