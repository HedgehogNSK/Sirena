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

    var creationFunction = () => new ValidateSirenaIdStep.Factory(Container.GetInstance<AskSirenaIdForRequestMessageBuilder.Factory>());
    RegisterStepFactoryIntoCommand<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>>
    (Lifestyle.Singleton, creationFunction);

    Container.Register< IFactory< NullableContainer<SirenRepresentation>, DeclineRequestStep>
      , DeclineRequestStep.Factory>(Lifestyle.Transient);
    Container.Register<DeclineRequestMessageBuilder>(Lifestyle.Transient);
    Container.Register<NotAllowedTextGetter.Factory>(Lifestyle.Singleton);
    Container.Register<NoRequestsTextGetter.Factory>(Lifestyle.Singleton);
    Container.Register<NoRequestorTextGetter.Factory>(Lifestyle.Singleton);
    Container.Register<SuccesfulDeclineTextGetter.Factory>(Lifestyle.Singleton);
  }
}