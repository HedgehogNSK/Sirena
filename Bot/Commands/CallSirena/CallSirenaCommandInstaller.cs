using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class CallSirenaCommandInstaller(Container container)
 : CommandInstaller<CallSirenaCommand>(container)
{
  public override void Install()
  {
    base.Install();

    Container.Register<NullableContainer<ObjectId>>();
    Container.Register<NullableContainer<SirenRepresentation>>();
    Container.Register<NullableContainer<Message>>();
    Container.Register<IFactory<IRequestContext,Container<IRequestContext>>,RequestContextContainerFactory>();

    Container.RegisterConditional<IFactory<IRequestContext, CommandPlan>, CallSirenaPlanFactory>((_predicate) 
      => _predicate.Consumer.ImplementationType == typeof(CallSirenaCommand));
  }
}
