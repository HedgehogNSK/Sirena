using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot.DI;

public class DeleteSirenaInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<DeleteSirenaCommand, DeleteSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenaData>, DeleteConcretteSirenaStep>, DeleteConcretteSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenaData, SuccesfulDeleteMessageBuilder>, SuccesfulDeleteMessageBuilder.Factory, DeleteConcretteSirenaStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenaData>, ConfirmationRemoveSirenaStep>, ConfirmationRemoveSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenaData, ConfirmRemoveSirenaMessageBuilder>, ConfirmRemoveSirenaMessageBuilder.Factory, ConfirmationRemoveSirenaStep.Factory>();
    Container.RegisterSingleton<IFactory<NullableContainer<SirenaData>, FindRemoveSirenaStep>, FindRemoveSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IncorrectParameterMessageBuilder>, IncorrectParameterMessageBuilder.Factory, FindRemoveSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IEnumerable<SirenaData>, RemoveSirenaMenuMessageBuilder>, RemoveSirenaMenuMessageBuilder.Factory, FindRemoveSirenaStep.Factory>();
  }
}