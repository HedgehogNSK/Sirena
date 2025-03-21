using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class CallSirenaCommandInstaller : PlanBassedCommandInstaller<CallSirenaCommand, CallSirenaPlanFactory>
{
  public CallSirenaCommandInstaller(Container container) : base(container) { }
  public override void Install()
  {
    base.Install();

    Container.RegisterSingleton<IFactory<NullableContainer<Message>, AddExtraInformationStep>, AddExtraInformationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, ISendMessageBuilder>, AddExtraInformationStep.MessagBuilderFactory, AddExtraInformationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, CheckCallAbilityStep>, CheckCallAbilityStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder>, StringNotIdMessageBuilder.Factory, CheckCallAbilityStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep>, SirenaExistensValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, string, ISendMessageBuilder>, SirenaExistensValidationStep.MessagBuilderFactory, SirenaExistensValidationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep>, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, int, SirenRepresentation, ISendMessageBuilder>, SirenaCallReportMessageBuilder.Factory, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<long,IRequestContext, SirenRepresentation, ISendMessageBuilder>, SirenaCallServiceMessageBuilder.Factory, CallSirenaStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep>, SirenaStateValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenRepresentation, ISendMessageBuilder>, NotAllowedToCallMessageBuilder.Factory, SirenaStateValidationStep.Factory>();
  }
}