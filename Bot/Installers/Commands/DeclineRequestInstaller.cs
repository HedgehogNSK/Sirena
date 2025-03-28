using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class DeclineRequestInstaller(Container container)
 : CommandInstaller<DeclineRequestCommand>(container)
{
  public override void Install()
  {
    base.Install();

    RegisterIntoCommand<IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep>
    , RequestsValidateSirenaIdStep.Factory>(Lifestyle.Singleton);
    
    Container.Register<IFactory<NullableContainer<SirenRepresentation>, DeclineRequestStep>
      , DeclineRequestStep.Factory>(Lifestyle.Singleton);
    Container.Register<DeclineRequestMessageBuilder.Factory>(Lifestyle.Singleton);
    Container.Register<NotAllowedTextGetter.Factory>(Lifestyle.Singleton);
    Container.Register<NoRequestsTextGetter.Factory>(Lifestyle.Singleton);
    Container.Register<NoRequestorTextGetter.Factory>(Lifestyle.Singleton);
    Container.Register<SuccesfulDeclineTextGetter.Factory>(Lifestyle.Singleton);
  }
}