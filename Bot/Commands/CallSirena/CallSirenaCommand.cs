using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public class CallSirenaCommand : PlanExecutorBotCommand
{
  public const string NAME = "call";
  public const string DESCRIPTION = "Call sirena by number or by id";

  public CallSirenaCommand(IFactory<IRequestContext,CommandPlan> planFactory
  , PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
}