using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeSirenaPlanFactory(
IFactory<NullableContainer<ObjectId>, ProcessParameterUnsubscribeStep> processParamUnsubscribeStep
  , IFactory<NullableContainer<ObjectId>, TryUnsubscribeStep> attemptUnsubscribeStep) 
  : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ObjectId> idContainer = new();
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      processParamUnsubscribeStep.Create(idContainer),
      attemptUnsubscribeStep.Create(idContainer)
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new(UnsubscribeCommand.NAME, [compositeStep]);
  }
}