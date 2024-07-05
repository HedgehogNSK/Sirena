using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class SharedCommandServicesInstaller(Container container) : Installer(container)
{
  public override void Install()
  {
    Container.Register<IFindSirenaOperation, SirenaOperations>();
    Container.Register<IUpdateSirenaOperation, SirenaOperations>();
  }
}