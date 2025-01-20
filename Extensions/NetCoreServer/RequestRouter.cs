using NetCoreServer;

namespace Hedgey.Extensions.NetCoreServer;

public class RequestRouter : IRequestRouter
{
  /// <summary>
  /// Contains http request handlers by relative pathes, e.g. "/home/"
  /// </summary>
  public Dictionary<string, IHTTPRequestHandler> handlers;
  static DestinationNotFoundHandler notFoundHandler = new DestinationNotFoundHandler();
  public RequestRouter(IEnumerable<KeyValuePair<string, IHTTPRequestHandler>>? routePairs = null)
  {
    handlers = routePairs == null ? new Dictionary<string, IHTTPRequestHandler>()
    : new Dictionary<string, IHTTPRequestHandler>(routePairs);
  }
  public RequestRouter Set(string route, IHTTPRequestHandler handler)
  {
    if (string.IsNullOrEmpty(route))
      route = "/";

    if (route[0] != '/')
      route = '/' + route;
    handlers[route] = handler;

    return this;
  }

  public IHTTPRequestHandler GetHandler(HttpRequest request)
  {
    var path = request.Url.Split('?', 2);
    return GetHandler(path[0]);
  }
  public IHTTPRequestHandler GetHandler(string route)
  {
    if (handlers.TryGetValue(route, out var handler))
      return handler;

    return notFoundHandler;
  }
  public IHTTPRequestHandler this[HttpRequest request]{
    get => GetHandler(request);
  }
  public IHTTPRequestHandler this[string route] {
    set => Set( route, value);
  }
}