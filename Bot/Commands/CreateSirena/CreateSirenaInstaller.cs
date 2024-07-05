using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaInstaller(Container container)
 : CommandInstaller<CreateSirenaCommand>(container)
{
  public override void Install()
  {
    base.Install();
    
    Container.Register<IGetUserOperationAsync,GetUserOperationAsync>();
    Container.Register<ICreateSirenaOperationAsync,CreateSirenaOperationAsync>();
    Container.RegisterConditional<IFactory<IRequestContext, CommandPlan>,CreateSirenaPlanFactory>((_predicate) 
      => _predicate.Consumer.ImplementationType == typeof(CreateSirenaCommand));
  }
}
