using Hedgey.Structure.Factory;
using NetCoreServer;

namespace Hedgey.Extensions.NetCoreServer
{
  public class HttpSession(global::NetCoreServer.HttpServer server, RequestRouter router)
  : global::NetCoreServer.HttpSession(server)
  {
    protected override void OnReceivedRequest(HttpRequest request)
    {
      var response = router.GetHandler(request).Handle(request);
      // Отправка ответа
      if (!SendResponseAsync(response))
        throw new Exception("Response hasn't been sent");
    }
    protected override void OnReceivedCachedRequest(HttpRequest request, byte[] content)
    {
      Console.WriteLine($"[{DateTime.Now}]: {request.Url}\nReceived cached request");
      base.OnReceivedCachedRequest(request, content);
    }
    /// <summary>
    /// Default factory
    /// </summary>
    /// <param name="router"></param>
    public class Factory(RequestRouter router)
      : IFactory<global::NetCoreServer.HttpServer, TcpSession>
    {
      private readonly RequestRouter router = router;

      public TcpSession Create(global::NetCoreServer.HttpServer server)
        => new HttpSession(server, router);
    }
  }
}