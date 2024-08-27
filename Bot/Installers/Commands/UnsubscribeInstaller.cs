using Hedgey.Extensions.SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class UnsubscribeInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<UnsubscribeCommand, UnsubscribeSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterStepFactoryWithBuilderFactory(typeof(ProcessParameterUnsubscribeStep.Factory), typeof(SubscriptionsMessageBuilderFactory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(TryUnsubscribeStep.Factory), typeof(UnsubscribeMessageBuilder.Factory));
  }
}