using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class StartCommand : PlanExecutorBotCommand
{
  public const string NAME = "start";
  public const string DESCRIPTION = "User initialization";
  public StartCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
}