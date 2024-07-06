using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;
public class CallSirenaCommand : PlanExecutorBotCommand
{
  public const string NAME = "call";
  public const string DESCRIPTION = "Call sirena by number or by id";

  public CallSirenaCommand(IFactory<IRequestContext, CommandPlan> planFactory
  , PlanScheduler planScheduler)
  : base(NAME, DESCRIPTION, planFactory, planScheduler)
  { }
  public class Installer(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<CallSirenaCommand, CallSirenaPlanFactory>(container)
  {
    public override void Install()
    {
      base.Install();

      Container.Register<NullableContainer<ObjectId>>();
      Container.Register<NullableContainer<SirenRepresentation>>();
      Container.Register<NullableContainer<Message>>();
    }
  }
}