using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class DisplaySirenaInfoInstaller(Container container)
 : PlanBassedCommandInstaller<DisplaySirenaInfoCommand, DisplaySirenaInfoPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.Register<AskSirenaIdForInfoMessageBuilder.Factory>();
    var creationFunction = () => new ValidateSirenaIdStep.Factory(Container.GetInstance<AskSirenaIdForInfoMessageBuilder.Factory>());
    RegisterStepFactoryIntoPlanFactory<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>>
      (Lifestyle.Singleton, creationFunction);

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, GetSirenaInfoStep>, GetSirenaInfoStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, ulong, ISendMessageBuilder>
    , SirenaInfoNotFoundMessageBuilder.Factory, GetSirenaInfoStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder>
    , SirenaInfoMessageBuilder.Factory, GetSirenaInfoStep.Factory>();
  }
}