using Hedgey.Extensions;
using Hedgey.Extensions.NetCoreServer;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Security.x509Certificates;
using Hedgey.Sirena.HTTP.Server;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.Setup;
using SimpleInjector;
using System.Reactive.Subjects;

namespace Hedgey.Sirena.Bot.DI.HTTP;

public class ServerInstaller(Container container) : Installer(container)
{
  public override void Install()
  {
    Container.Register<IFactory<NetCoreServer.HttpServer>, ServerFactory>();
    Container.RegisterSingleton(() => Container.GetInstance<IFactory<NetCoreServer.HttpServer>>().Create());
    Container.Register<IFactory<NetCoreServer.HttpServer, NetCoreServer.TcpSession>, HttpSession.Factory>();

    Container.Register<ICertificateProvider, X509StoreCertificateProvider>();
    Container.Register<IFactory<HttpsServer>, ServerFactory>();
    Container.RegisterSingleton(() => Container.GetInstance<IFactory<HttpsServer>>().Create());
    Container.Register<IFactory<NetCoreServer.HttpsServer, NetCoreServer.SslSession>, HttpsSession.Factory>();

    Container.RegisterSingleton(() => new RequestRouter(null));

    Container.Register<ISubject<Update>, Subject<Update>>(Lifestyle.Singleton);
    Container.Register<IObservable<Update>, Subject<Update>>(Lifestyle.Singleton);
    Container.Register<UpdateHandler>();
    Container.RegisterSingleton<Uri>(() =>
    {
      var guid = Guid.NewGuid();
      string url = OSTools.GetEnvironmentVar("SIRENA_WH_URL") + guid;
      return new Uri(url);
    });
    Container.RegisterInitializer<UpdateHandler>(_updateHander =>
    {
      var route = Container.GetInstance<Uri>().AbsolutePath;
      Container.GetInstance<RequestRouter>()[route] = _updateHander;
    });

    Container.RegisterSingleton<SetWebhook>(() =>
    {
      var url = Container.GetInstance<Uri>().AbsoluteUri;
      Console.WriteLine($"Setting webhook to url: {url}");
      return new SetWebhook()
      {
        Url = url,
        AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
      };
    });
  }
}