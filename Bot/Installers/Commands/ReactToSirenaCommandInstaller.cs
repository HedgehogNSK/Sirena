using Hedgey.Structure.Factory;
using MongoDB.Bson;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class ReactToSirenaCommandInstaller(Container container)
   : CommandInstaller<ReactToSirenaCommand>(container)
{
  public override void Install()
  {
    base.Install();

    RegisterIntoCommand<IFactory<NullableContainer<ObjectId>, SetReactionStep>, SetReactionStep.Factory>(Lifestyle.Singleton);
  }
}