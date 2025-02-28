using Hedgey.Extensions.SimpleInjector;
using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class StartInstaller(Container container)
   : PlanBassedCommandInstaller<StartCommand, StartPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    var creationFunction = () => new ValidateSirenaIdStep.Factory(Container.GetInstance<AskSirenaIdMessageBuilder.Factory>());
    RegisterStepFactoryIntoPlanFactory<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>>
    (Lifestyle.Singleton, creationFunction);

    Container.RegisterStepFactoryWithBuilderFactory(typeof(WelcomeMessageStep.Factory), typeof(WelcomeMessageBuilderFactory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(CreateUserStep.Factory), typeof(CreateUserFailedMessageBuilder.Factory));
  }
}