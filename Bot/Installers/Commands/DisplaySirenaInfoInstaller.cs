using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot.DI;

public class DisplaySirenaInfoInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<DisplaySirenaInfoCommand, DisplaySirenaInfoPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>, ValidateSirenaIdStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IMessageBuilder>
    , AskSirenaIdMessageBuilder.Factory, ValidateSirenaIdStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, GetSirenaInfoStep>, GetSirenaInfoStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, ulong, SirenaNotFoundMessageBuilder>
    , SirenaNotFoundMessageBuilder.Factory, GetSirenaInfoStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder>
    , SirenaInfoMessageBuilder.Factory, GetSirenaInfoStep.Factory>();
  }
}