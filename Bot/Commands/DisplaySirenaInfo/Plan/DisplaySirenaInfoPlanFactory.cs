using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class DisplaySirenaInfoPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly Container container;

  public DisplaySirenaInfoPlanFactory(Container container)
  {
    this.container = container;
  }

  public CommandPlan Create(IRequestContext context)
  {
    CommandStep[] steps = [
      container.GetInstance<ValidateSirenaIdStep>(),
      container.GetInstance<GetSirenaInfoStep>()
    ];
    return new(DisplaySirenaInfoCommand.NAME, steps);
  }
}