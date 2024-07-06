using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaCommand : PlanExecutorBotCommand
{
  public const string NAME = "create";
  public const string DESCRIPTION = "Creates a sirena with certain title";

  public CreateSirenaCommand(IFactory<IRequestContext, CommandPlan> planFactory
    , PlanScheduler planScheduler)
   : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
public class Installer(SimpleInjector.Container container)
 :PlanBassedCommandInstaller<CreateSirenaCommand,CreateSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();
    
    Container.Register<IGetUserOperationAsync,GetUserOperationAsync>();
    Container.Register<ICreateSirenaOperationAsync,CreateSirenaOperationAsync>();
  }
}
}