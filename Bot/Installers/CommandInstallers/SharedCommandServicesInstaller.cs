using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class SharedCommandServicesInstaller(Container container) : Installer(container)
{
  public override void Install()
  {
    Container.Register<IDeleteSirenaOperation, SirenaOperations>();
    Container.Register<IFindSirenaOperation, SirenaOperations>();
    Container.Register<IGetUserRelatedSirenas, SirenaOperations>();
    Container.Register<ISubscribeToSirenaOperation, SirenaOperations>();
    Container.Register<IUpdateSirenaOperation, SirenaOperations>();
    Container.Register<IUnsubscribeSirenaOperation, SirenaOperations>();
  }
}