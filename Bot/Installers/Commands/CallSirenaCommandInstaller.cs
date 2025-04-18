using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Entities;
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
    Container.RegisterConditional<IFactory<IRequestContext, IEnumerable<SirenaData>, ISendMessageBuilder>, StringNotIdMessageBuilder.Factory, CheckCallAbilityStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, NullableContainer<SirenaData>, SirenaExistensValidationStep>, SirenaExistensValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, string, ISendMessageBuilder>, SirenaExistensValidationStep.MessagBuilderFactory, SirenaExistensValidationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenaData>, NullableContainer<Message>, CallSirenaStep>, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, int, SirenaData, ISendMessageBuilder>, SirenaCallReportMessageBuilder.Factory, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, ServiceMessageData, ISendMessageBuilder>, SirenaCallServiceMessageBuilder.Factory, CallSirenaStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenaData>, SirenaStateValidationStep>, SirenaStateValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenaData, ISendMessageBuilder>, NotAllowedToCallMessageBuilder.Factory, SirenaStateValidationStep.Factory>();
  }
}