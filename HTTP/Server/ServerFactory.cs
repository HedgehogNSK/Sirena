using Hedgey.Extensions;
using Hedgey.Security.x509Certificates;
using Hedgey.Structure.Factory;
using NetCoreServer;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Hedgey.Sirena.HTTP.Server;

public class ServerFactory(ICertificateProvider provider
  , IFactory<HttpServer, TcpSession> tcpSessionFactory
  , IFactory<HttpsServer, SslSession> sslSessionFactory)
   : IFactory<Extensions.NetCoreServer.HttpServer>, IFactory<Extensions.NetCoreServer.HttpsServer>
{
  private readonly ICertificateProvider provider = provider;
  private readonly IFactory<HttpServer, TcpSession> tcpSessionFactory = tcpSessionFactory;
  private readonly IFactory<HttpsServer, SslSession> sslSessionFactory = sslSessionFactory;

  static System.Net.IPAddress Address => System.Net.IPAddress.Any;
  static int Port => int.Parse(OSTools.GetEnvironmentVar("SIRENA_PORT"));

  public Extensions.NetCoreServer.HttpServer Create()
    => new Extensions.NetCoreServer.HttpServer(Address, Port, tcpSessionFactory);

  Extensions.NetCoreServer.HttpsServer IFactory<Extensions.NetCoreServer.HttpsServer>.Create()
  {
    X509Certificate2 certificate = provider.Get();
    Console.WriteLine("SSL Certificate: " + certificate.FriendlyName);
    Console.WriteLine(certificate.Thumbprint);    
    Console.WriteLine($"Certificate subject: {certificate.Subject}");
    Console.WriteLine($"Valid from: {certificate.NotBefore}");
    Console.WriteLine($"Valid until: {certificate.NotAfter}");
    var context = new SslContext(SslProtocols.Tls12 | SslProtocols.Tls13, certificate);
    Extensions.NetCoreServer.HttpsServer server = new Extensions.NetCoreServer.HttpsServer(context, Address, Port, sslSessionFactory);
    return server;
  }
}