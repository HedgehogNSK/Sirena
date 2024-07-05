using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaInstaller(Container container)
 : CommandInstaller<DeleteSirenaCommand>(container)
{
  public override void Install()
  {
    base.Install();
    
    Container.RegisterConditional<IFactory<IRequestContext, CommandPlan>,DeleteSirenaPlanFactory>((_predicate) 
      => _predicate.Consumer.ImplementationType == typeof(DeleteSirenaCommand));
  }
}
