using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class CallSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFactory<NullableContainer<ObjectId>, SirenaIdValidationStep> idValidationStepFactory;
  private readonly IFactory<NullableContainer<Message>, AddExtraInformationStep> addExtraInfoStepFactory;
  private readonly IFactory<NullableContainer<ObjectId>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep> isSirenaExistStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep> callSirenaStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep> sirenaStateValidationStepFactory;

  public CallSirenaPlanFactory(IFactory<NullableContainer<ObjectId>, SirenaIdValidationStep> idValidationStepFactory
  , IFactory<NullableContainer<Message>, AddExtraInformationStep> addExtraInfoStepFactory
  , IFactory<NullableContainer<ObjectId>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep> isSirenaExistStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, NullableContainer<Message>, CallSirenaStep> callSirenaStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, SirenaStateValidationStep> sirenaStateValidationStepFactory)
  {
    this.idValidationStepFactory = idValidationStepFactory;
    this.addExtraInfoStepFactory = addExtraInfoStepFactory;
    this.isSirenaExistStepFactory = isSirenaExistStepFactory;
    this.callSirenaStepFactory = callSirenaStepFactory;
    this.sirenaStateValidationStepFactory = sirenaStateValidationStepFactory;
  }

  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ObjectId> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContainer = new();
    NullableContainer<Message> messageContainer = new();
    //Create composition step from different validations
    //Because we have to make all validations for each itteration of input data
    CompositeCommandStep compositeStep = new(
      idValidationStepFactory.Create(idContainer),
      isSirenaExistStepFactory.Create(idContainer, sirenaContainer),
      sirenaStateValidationStepFactory.Create(sirenaContainer)
    );
    IObservableStep<IRequestContext, CommandStep.Report>[] steps =
    [
      compositeStep,
      addExtraInfoStepFactory.Create(messageContainer),
      callSirenaStepFactory.Create(sirenaContainer,messageContainer)
      ];
    return new(CallSirenaCommand.NAME, steps);
  }
}