using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class PlanExecutorBotCommand : AbstractBotCommmand
 {
  private readonly IFactory<IRequestContext, CommandPlan> planFactory;
  private readonly PlanScheduler planScheduler;

  public PlanExecutorBotCommand(string name, string description
  , IFactory<IRequestContext,CommandPlan> planFactory, PlanScheduler planScheduler)
   : base(name, description)
  {
    this.planFactory = planFactory;
    this.planScheduler = planScheduler;
  }
  public PlanExecutorBotCommand(string name, string description,  PlanScheduler planScheduler)
   : base(name, description)
  {
    this.planScheduler = planScheduler;
  }

   public sealed override void Execute(IRequestContext context)
  {
    var plan = Create(context);
    planScheduler.Push(plan, context);
  }
  protected virtual CommandPlan Create(IRequestContext context)
    => planFactory.Create(context);
}