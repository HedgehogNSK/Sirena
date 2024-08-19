using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class PlanExecutorBotCommand : AbstractBotCommmand
 {
   private readonly IFactory<IRequestContext,CommandPlan> planFactory;
  private readonly PlanScheduler planScheduler;

  public PlanExecutorBotCommand(string name, string description
  , IFactory<IRequestContext,CommandPlan> planFactory, PlanScheduler planScheduler)
   : base(name, description)
  {
    this.planFactory = planFactory;
    this.planScheduler = planScheduler;
  }

  public override void Execute(IRequestContext context)
  {
    var plan = planFactory.Create(context);
    planScheduler.Push(plan, context);
  }
}