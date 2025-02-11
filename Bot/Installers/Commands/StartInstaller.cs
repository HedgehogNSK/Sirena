using Hedgey.Extensions.SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class StartInstaller(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<StartCommand, StartPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterStepFactoryWithBuilderFactory(typeof(WelcomeMessageStep.Factory), typeof(WelcomeMessageBuilderFactory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(CreateUserStep.Factory), typeof(CreateUserFailedMessageBuilder.Factory));

  }
}
