using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Extensions.SimpleInjector;
using MongoDB.Bson;
namespace Hedgey.Sirena.Bot.DI;

public class DisplaySirenaInfoInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<DisplaySirenaInfoCommand, DisplaySirenaInfoPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterSingleton<IFactory<NullableContainer<ObjectId>, ValidateSirenaIdStep>, ValidateSirenaIdStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IMessageBuilder>
    , AskSirenaIdMessageBuilder.Factory, ValidateSirenaIdStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ObjectId>, GetSirenaInfoStep>, GetSirenaInfoStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, ObjectId, SirenaNotFoundMessageBuilder>
    , SirenaNotFoundMessageBuilder.Factory, GetSirenaInfoStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder>
    , SirenaInfoMessageBuilder.Factory, GetSirenaInfoStep.Factory>();
  }
}