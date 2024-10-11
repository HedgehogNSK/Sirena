using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class RequestRightsPlanFactory(IFactory<NullableContainer<ulong>, ValidateSirenaIdStep> validateIdStepFactory
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>
  , SirenaExistensValidationStep> sirenExistensValidationStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, AddRequestMessageStep> addRequestMessageStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, SendRequestStep> sendRequestStepFactory
  ,  IFactory<DisplayCommandMenuStep> displayCommandMenuStepFactory) 
  : IFactory<IRequestContext, CommandPlan>
{

  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ulong> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContainer = new();
    CompositeCommandStep validationBulkStep = new(
      validateIdStepFactory.Create(idContainer),
      sirenExistensValidationStepFactory.Create(idContainer, sirenaContainer)
    );

    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      displayCommandMenuStepFactory.Create(),
      validationBulkStep,
      addRequestMessageStepFactory.Create(sirenaContainer),
      sendRequestStepFactory.Create(sirenaContainer)
    ];
    return new(RequestRightsCommand.NAME, steps);
  }
}