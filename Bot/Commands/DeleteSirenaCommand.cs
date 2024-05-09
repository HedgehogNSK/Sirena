using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;
public class DeleteSirenaCommand : PlanExecutorBotCommand
{
  public const string NAME = "delete";
  public const string DESCRIPTION = "Delete your Sirena by ID or by Serial number.";
  public DeleteSirenaCommand(IFactory<IRequestContext, CommandPlan> planFactory, PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler) { }
}