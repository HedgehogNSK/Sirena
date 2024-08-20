using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
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

    Container.RegisterSingleton<IFactory<NullableContainer<ObjectId>, SirenaIdValidationStep>, SirenaIdValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, string, IMessageBuilder>, StringNotIdMessageBuilder.Factory, SirenaIdValidationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<ObjectId>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep>, SirenaExistensValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, ObjectId, IMessageBuilder>, SirenaExistensValidationStep.MessagBuilderFactory, SirenaExistensValidationStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep>, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, int, SirenRepresentation, IMessageBuilder>, SirenaCallReportMessageBuilder.Factory, CallSirenaStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>, SirenaCallServiceMessageBuilder.Factory, CallSirenaStep.Factory>();

    Container.RegisterSingleton<IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep>, SirenaStateValidationStep.Factory>();
    Container.RegisterConditional<IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>, NotAllowedToCallMessageBuilder.Factory, SirenaStateValidationStep.Factory>();
  }
}