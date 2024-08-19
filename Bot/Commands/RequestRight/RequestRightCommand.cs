using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;
public class RequestRightsCommand : PlanExecutorBotCommand
{
  public const string NAME = "request";
  public const string DESCRIPTION = "Request owner of sirena to delegate right to call sirena";
  public RequestRightsCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
   : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }

  public class Installer(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<RequestRightsCommand, RequestRightsPlanFactory>(container)
  {
    public override void Install()
    {
      base.Install();

      Container.RegisterConditional<IFactory<IRequestContext, IRightsRequestOperation.Result, IMessageBuilder>, RightRequestResultMessageBuilder.Factory>(
        _context => _context.Consumer.ImplementationType == typeof(SendRequestStep)
      );
    }
  }
}