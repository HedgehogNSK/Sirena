using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
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
    Container.RegisterConditional<IFactory<IRequestContext, IMessageBuilder>, AddExtraInformationStep.MessagBuilderFactory, AddExtraInformationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, SirenaIdValidationStep>, SirenaIdValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, IEnumerable<SirenRepresentation>, IMessageBuilder>, StringNotIdMessageBuilder.Factory, SirenaIdValidationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep>, SirenaExistensValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, string, IMessageBuilder>, SirenaExistensValidationStep.MessagBuilderFactory, SirenaExistensValidationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep>, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, int, SirenRepresentation, IMessageBuilder>, SirenaCallReportMessageBuilder.Factory, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<long,IRequestContext, SirenRepresentation, IMessageBuilder>, SirenaCallServiceMessageBuilder.Factory, CallSirenaStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep>, SirenaStateValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>, NotAllowedToCallMessageBuilder.Factory, SirenaStateValidationStep.Factory>();
  }
}