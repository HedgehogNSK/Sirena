using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class PlanBassedCommandInstaller<TCommand, TPlanFactory>(Container container)
 : CommandInstaller<TCommand>(container)
 where TCommand : PlanExecutorBotCommand
 where TPlanFactory : class, IFactory<IRequestContext, CommandPlan>
{
  public override void Install()
  {
    base.Install();

    Container.RegisterConditional<IFactory<IRequestContext, CommandPlan>, TPlanFactory>((_predicate)
      => _predicate.Consumer.ImplementationType == typeof(TCommand));
  }
}