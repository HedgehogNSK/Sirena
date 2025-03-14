using NetCoreServer;
using Newtonsoft.Json;

namespace Hedgey.Extensions.NetCoreServer;

static public class HttpExtension
{
  static public T Parse<T>(HttpRequest request, JsonSerializerSettings? settings = null)
  {
    var serializer = JsonSerializer.Create(settings);
    var stringReader = new StringReader(request.Body);
    var jsonReader = new JsonTextReader(stringReader);
    T update = serializer.Deserialize<T>(jsonReader)
     ?? throw new ArgumentException($"HTTP request body is not {typeof(T).Name}\n Request body: {request.Body}");
    return update;
  }
}