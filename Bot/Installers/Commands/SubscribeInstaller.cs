using Hedgey.Extensions.SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class SubscribeInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<SubscribeCommand, SubscribeSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterStepFactoryWithBuilderFactories(typeof(RequestSubscribeStep.Factory)
    , [typeof(SuccesfulSubscriptionMessageBuilder.Factory), typeof(SirenaNotFoundMessageBuilder.Factory)]);
  }
}