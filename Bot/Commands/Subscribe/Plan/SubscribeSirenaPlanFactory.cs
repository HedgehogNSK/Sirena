using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class SubscribeSirenaPlanFactory(IFactory<NullableContainer<ulong>, ValidateSirenaIdStep> idValidationStepFactory
, IFactory<NullableContainer<ulong>, RequestSubscribeStep> requestSubscribeStepFactory)
 : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ulong> idContainer = new();
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      idValidationStepFactory.Create(idContainer),
      requestSubscribeStepFactory.Create(idContainer),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new(SubscribeCommand.NAME, [compositeStep]);
  }
}