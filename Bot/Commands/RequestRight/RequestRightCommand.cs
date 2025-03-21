using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public class RequestRightsCommand : PlanExecutorBotCommand
{
  public const string NAME = "request";
  public const string DESCRIPTION = "Request owner of sirena to delegate right to call sirena";
  public RequestRightsCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
   : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
}