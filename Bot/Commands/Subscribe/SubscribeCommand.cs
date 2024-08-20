using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class SubscribeCommand : PlanExecutorBotCommand
{
  public const string NAME = "subscribe";
  public const string DESCRIPTION = "Subscribes to *sirena* by id.";
  public SubscribeCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
}