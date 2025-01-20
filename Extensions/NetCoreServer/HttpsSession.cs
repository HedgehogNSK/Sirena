using Hedgey.Structure.Factory;
using NetCoreServer;

namespace Hedgey.Extensions.NetCoreServer
{
  public class HttpsSession(global::NetCoreServer.HttpsServer server,RequestRouter router)
  : global::NetCoreServer.HttpsSession(server)
  {
    protected override void OnReceivedRequest(HttpRequest request)
    {
      var response = router.GetHandler(request).Handle(request);
      // Отправка ответаw
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
    public class Factory( RequestRouter router) 
      : IFactory<global::NetCoreServer.HttpsServer ,SslSession>
    {
      private readonly RequestRouter router = router;
      public SslSession Create(global::NetCoreServer.HttpsServer server) 
        => new HttpsSession(server, router);
    }
  }
}