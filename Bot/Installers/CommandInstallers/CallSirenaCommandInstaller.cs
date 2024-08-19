using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CallSirenaCommandInstaller : PlanBassedCommandInstaller<CallSirenaCommand, CallSirenaPlanFactory>
{
  public CallSirenaCommandInstaller(Container container) : base(container) { }
  public override void Install()
  {
    base.Install();

    Container.Register<IFactory<NullableContainer<Message>, AddExtraInformationStep>, AddExtraInformationStep.Factory>();
    RegisterMessageBuilderFactoryForStepFactory<IFactory<IRequestContext, IMessageBuilder>, AddExtraInformationStep.MessagBuilderFactory, AddExtraInformationStep.Factory>(Container);

    Container.Register<IFactory<NullableContainer<ObjectId>, SirenaIdValidationStep>, SirenaIdValidationStep.Factory>();
    RegisterMessageBuilderFactoryForStepFactory<IFactory<IRequestContext, string, IMessageBuilder>, StringNotIdMessageBuilder.Factory, SirenaIdValidationStep.Factory>(Container);

    Container.Register<IFactory<NullableContainer<ObjectId>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep>, SirenaExistensValidationStep.Factory>();
    RegisterMessageBuilderFactoryForStepFactory<IFactory<IRequestContext, ObjectId, IMessageBuilder>, SirenaExistensValidationStep.MessagBuilderFactory, SirenaExistensValidationStep.Factory>(Container);

    Container.Register<IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep>, CallSirenaStep.Factory>();
    RegisterMessageBuilderFactoryForStepFactory<IFactory<IRequestContext, int, SirenRepresentation, IMessageBuilder>, SirenaCallReportMessageBuilder.Factory, CallSirenaStep.Factory>(Container);
    RegisterMessageBuilderFactoryForStepFactory<IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>, SirenaCallServiceMessageBuilder.Factory, CallSirenaStep.Factory>(Container);

    Container.Register<IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep>, SirenaStateValidationStep.Factory>();
    RegisterMessageBuilderFactoryForStepFactory<IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>, NotAllowedToCallMessageBuilder.Factory, SirenaStateValidationStep.Factory>(Container);
  }

  /// <summary>
  /// Shortcut for conditional registration message builder factories into dependent step factories
  /// </summary>
  /// <typeparam name="IMBFactory">Interface of the message builder factory</typeparam>
  /// <typeparam name="MBFactory">message builder factory implementation type</typeparam>
  /// <typeparam name="StepFactory">step factory implementation type</typeparam>
  /// <param name="container"></param>
  static public void RegisterMessageBuilderFactoryForStepFactory<IMBFactory, MBFactory, StepFactory>(Container container)
    where IMBFactory : class
    where MBFactory : class, IMBFactory
  {
    container.RegisterConditional<IMBFactory, MBFactory>(c => c.Consumer.ImplementationType == typeof(StepFactory));
  }
}