using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaCommand : AbstractBotCommmand
{
  const string NAME = "create";
  const string DESCRIPTION = "Creates a sirena with certain title. Example: `/create Sirena`";
  private readonly IFactory<IRequestContext, CommandPlan> planFactory;
  private readonly PlanScheduler planScheduler;

  public CreateSirenaCommand(IFactory<IRequestContext, CommandPlan> planFactory,
PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION)
  {
    this.planFactory = planFactory;
    this.planScheduler = planScheduler;
  }
  public override void Execute(IRequestContext context)
  {
    var plan = planFactory.Create(context);
    planScheduler.Push(plan);
  }
}