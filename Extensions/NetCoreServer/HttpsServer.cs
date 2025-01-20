using Hedgey.Structure.Factory;
using NetCoreServer;
using System.Net;

namespace Hedgey.Extensions.NetCoreServer
{
  public class HttpsServer(SslContext context, IPAddress address
    , int port, IFactory<global::NetCoreServer.HttpsServer,SslSession> sessionFactory)
   : global::NetCoreServer.HttpsServer(context, address, port)
  {
    private readonly IFactory<global::NetCoreServer.HttpsServer,SslSession> sessionFactory = sessionFactory;

    protected override SslSession CreateSession() 
      => sessionFactory.Create(this);
  }
}