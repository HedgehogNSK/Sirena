using NetCoreServer;

namespace Hedgey.Extensions.NetCoreServer;

public interface IHTTPRequestHandler
{
  HttpResponse Handle(HttpRequest request);
}