using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class FindSirenaCommand : PlanExecutorBotCommand
{
  public const string NAME = "find";
  public const string DESCRIPTION = "Find sirena by key string";

  public FindSirenaCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  {
  }
}