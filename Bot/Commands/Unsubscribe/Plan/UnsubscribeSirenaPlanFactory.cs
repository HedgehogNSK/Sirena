using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeSirenaPlanFactory(
IFactory<NullableContainer<ulong>, ProcessParameterUnsubscribeStep> processParamUnsubscribeStep
  , IFactory<NullableContainer<ulong>, TryUnsubscribeStep> attemptUnsubscribeStep) 
  : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ulong> idContainer = new();
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      processParamUnsubscribeStep.Create(idContainer),
      attemptUnsubscribeStep.Create(idContainer)
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new(UnsubscribeCommand.NAME, [compositeStep]);
  }
}