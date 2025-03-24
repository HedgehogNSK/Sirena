using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class PlanBassedCommandInstaller<TCommand, TPlanFactory>(Container container)
 : CommandInstaller<TCommand>(container)
 where TCommand : PlanExecutorBotCommand
 where TPlanFactory : class, IFactory<IRequestContext, CommandPlan>
{
  public override void Install()
  {
    base.Install();

    Container.RegisterConditional<IFactory<IRequestContext, CommandPlan>, TPlanFactory>(
      Lifestyle.Singleton
    ,(_predicate) => _predicate.Consumer.ImplementationType == typeof(TCommand)
    );
  }
  protected void RegisterStepFactoryIntoPlanFactory<TStepInterface>
  (Lifestyle lifestyle, Func<TStepInterface> instanceCreator)
  where TStepInterface : class
  {
    var registration = lifestyle.CreateRegistration(instanceCreator, Container);
    Container.RegisterConditional<TStepInterface>(registration
      , (_context) => _context.Consumer.ImplementationType == typeof(TPlanFactory));
  }
  protected void RegisterStepFactoryIntoPlanFactory<TStepInterface>
  (Lifestyle lifestyle, Type concretteType)
  where TStepInterface : class
  {
    var registration = lifestyle.CreateRegistration(concretteType, Container);
    Container.RegisterConditional<TStepInterface>(registration
      , (_context) => _context.Consumer.ImplementationType == typeof(TPlanFactory));
  }
  protected void RegisterStepFactoryIntoPlanFactory<TStepInterface, TStepImpl>
  (Lifestyle lifestyle)
  where TStepInterface : class
  where TStepImpl : TStepInterface
  => RegisterStepFactoryIntoPlanFactory<TStepInterface>(lifestyle, typeof(TStepImpl));
}