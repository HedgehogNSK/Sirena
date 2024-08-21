using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class SubscribeSirenaPlanFactory(IFactory<NullableContainer<ObjectId>, ValidateSirenaIdStep> idValidationStepFactory
, IFactory<NullableContainer<ObjectId>, RequestSubscribeStep> requestSubscribeStepFactory)
 : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ObjectId> idContainer = new();
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      idValidationStepFactory.Create(idContainer),
      requestSubscribeStepFactory.Create(idContainer),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new(SubscribeCommand.NAME, [compositeStep]);
  }
}