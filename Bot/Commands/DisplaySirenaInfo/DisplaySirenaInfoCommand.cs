using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;
public class DisplaySirenaInfoCommand : PlanExecutorBotCommand
{
  public const string NAME = "info";
  public const string DESCRIPTION = "Display sirena detailed information and available commands for itself.";
  public DisplaySirenaInfoCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  {
  }
}