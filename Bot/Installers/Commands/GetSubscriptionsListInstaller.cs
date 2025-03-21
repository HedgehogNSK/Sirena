using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class GetSubscriptionsListInstaller(Container container)
   : CommandInstaller<GetSubscriptionsListCommand>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterConditional<IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder>
    ,SubscriptionsMessageBuilderFactory>(Lifestyle.Singleton,
      x => x.Consumer.ImplementationType == typeof(GetSubscriptionsListCommand)
    );
  }
}
