using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CallSirenaCommandInstaller(Container container)
 :PlanBassedCommandInstaller<CallSirenaCommand,CallSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.Register<NullableContainer<ObjectId>>();
    Container.Register<NullableContainer<SirenRepresentation>>();
    Container.Register<NullableContainer<Message>>();
    Container.Register<IFactory<IRequestContext,Container<IRequestContext>>,RequestContextContainerFactory>();
  }
}
