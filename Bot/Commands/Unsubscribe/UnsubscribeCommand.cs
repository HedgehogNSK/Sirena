using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeCommand : PlanExecutorBotCommand
{
  public const string NAME = "unsubscribe";
  public const string DESCRIPTION = "Unsubscribes from certain sirena.";

  public UnsubscribeCommand(IFactory<IRequestContext, CommandPlan> planFactory, PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
  public class Installer(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<UnsubscribeCommand, UnsubscribeSirenaPlanFactory>(container)
  { }
}