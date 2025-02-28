using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class RequestsCommand : PlanExecutorBotCommand
{
  public const string NAME = "requests";
  public const string DESCRIPTION = "Display a list of requests for permission to launch a sirena.";
  public RequestsCommand(IFactory<IRequestContext, CommandPlan> planFactory, PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  {
  }
}