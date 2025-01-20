using Hedgey.Structure.Factory;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;

namespace Hedgey.Extensions.NetCoreServer
{
  public class HttpServer(IPAddress address, int port, IFactory<global::NetCoreServer.HttpServer,TcpSession> sessionFactory) 
  : global::NetCoreServer.HttpServer(address, port)
  {
    private readonly IFactory<global::NetCoreServer.HttpServer,TcpSession> sessionFactory = sessionFactory;

    protected override TcpSession CreateSession() 
      => sessionFactory.Create(this);

    protected override void OnError(SocketError error)
    {
      Console.WriteLine($"Chat TCP server caught an error with code {error}");
    }
  }
}