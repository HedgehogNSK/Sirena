using NetCoreServer;

namespace Hedgey.Extensions.NetCoreServer;

public interface IRequestRouter
{
  IHTTPRequestHandler GetHandler(HttpRequest request);
  IHTTPRequestHandler GetHandler(string route);
}