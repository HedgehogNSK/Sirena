using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly Container container;

  public UnsubscribeSirenaPlanFactory(Container container)
  {
    this.container = container;
  }

  public CommandPlan Create(IRequestContext context)
  {
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      container.GetInstance<ProcessParameterUnsubscribeStep>(),
      container.GetInstance<TryUnsubscribeStep>()
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new(UnsubscribeCommand.NAME, [compositeStep]);
  }
}