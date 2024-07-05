using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaInstaller(Container container)
 :PlanBassedCommandInstaller<CreateSirenaCommand,CreateSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();
    
    Container.Register<IGetUserOperationAsync,GetUserOperationAsync>();
    Container.Register<ICreateSirenaOperationAsync,CreateSirenaOperationAsync>();
  }
}
