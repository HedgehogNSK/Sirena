using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly Container container;

  public DeleteSirenaPlanFactory(Container container)
  {
    this.container = container;
  }

  public CommandPlan Create(IRequestContext context)
  {
    CommandStep[] steps = [
      container.GetInstance<FindRemoveSirenaStep>(),
      container.GetInstance<ConfirmationRemoveSirenaStep>(),
      container.GetInstance<DeleteConcretteSirenaStep>()
    ];
    return new(DeleteSirenaCommand.NAME, steps);
  }
}