using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class SubscribeSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly Container container;

  public SubscribeSirenaPlanFactory(Container container)
  {
    this.container = container;
  }

  public CommandPlan Create(IRequestContext context)
  {
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      container.GetInstance<ValidateSirenaIdStep>(),
      container.GetInstance<RequestSubscribeStep>()
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new(SubscribeCommand.NAME, [compositeStep]);
  }
}