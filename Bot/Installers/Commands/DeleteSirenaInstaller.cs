using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot.DI;

public class DeleteSirenaInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<DeleteSirenaCommand, DeleteSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, DeleteConcretteSirenaStep>, DeleteConcretteSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenRepresentation, SuccesfulDeleteMessageBuilder>, SuccesfulDeleteMessageBuilder.Factory, DeleteConcretteSirenaStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, ConfirmationRemoveSirenaStep>, ConfirmationRemoveSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenRepresentation, ConfirmRemoveSirenaMessageBuilder>, ConfirmRemoveSirenaMessageBuilder.Factory, ConfirmationRemoveSirenaStep.Factory>();
    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, FindRemoveSirenaStep>, FindRemoveSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IncorrectParameterMessageBuilder>, IncorrectParameterMessageBuilder.Factory, FindRemoveSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IEnumerable<SirenRepresentation>, RemoveSirenaMenuMessageBuilder>, RemoveSirenaMenuMessageBuilder.Factory, FindRemoveSirenaStep.Factory>();
  }
}