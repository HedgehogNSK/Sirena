using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeCommand : PlanExecutorBotCommand
{
  public const string NAME = "unsubscribe";
  public const string DESCRIPTION = "Unsubscribes from certain sirena.";

  public UnsubscribeCommand(IFactory<IRequestContext, CommandPlan> planFactory, PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
}