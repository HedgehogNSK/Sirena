using Hedgey.Extensions.SimpleInjector;
using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class SubscribeInstaller(Container container)
 : PlanBassedCommandInstaller<SubscribeCommand, SubscribeSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.Register<AskSirenaIdMessageBuilder.Factory>();
    var creationFunction = () => new ValidateSirenaIdStep.Factory(Container.GetInstance<AskSirenaIdMessageBuilder.Factory>());
    RegisterStepFactoryIntoPlanFactory<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>>
    (Lifestyle.Singleton, creationFunction);

    Container.RegisterStepFactoryWithBuilderFactories(typeof(RequestSubscribeStep.Factory)
    , [typeof(SuccesfulSubscriptionMessageBuilder.Factory), typeof(SirenaNotFoundMessageBuilder.Factory)]);
  }
}