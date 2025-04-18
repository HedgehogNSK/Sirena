using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestRightsPlanFactory(IFactory<NullableContainer<ulong>, ValidateSirenaIdStep> validateIdStepFactory
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenaData>, SirenaExistensValidationStep> sirenExistensValidationStepFactory
  , IFactory<NullableContainer<SirenaData>, AddRequestMessageStep> addRequestMessageStepFactory
  , IFactory<NullableContainer<SirenaData>, SendRequestStep> sendRequestStepFactory
  ,  IFactory<DisplayCommandMenuStep> displayCommandMenuStepFactory) 
  : IFactory<IRequestContext, CommandPlan>
{

  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ulong> idContainer = new();
    NullableContainer<SirenaData> sirenaContainer = new();
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