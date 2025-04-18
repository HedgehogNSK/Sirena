using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.MongoDB.Operations;
using Hedgey.Structure.Factory;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class RequestRightsInstaller(Container container)
   : PlanBassedCommandInstaller<RequestRightsCommand, RequestRightsPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.Register<AskSirenaIdForRequestMessageBuilder.Factory>();
    var creationFunction = () => new ValidateSirenaIdStep.Factory(Container.GetInstance<AskSirenaIdForRequestMessageBuilder.Factory>());
    RegisterIntoPlanFactory<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>>
    (Lifestyle.Singleton, creationFunction);

    Container.RegisterStepFactoryWithBuilderFactory(typeof(DisplayCommandMenuStep.Factory), typeof(RequestListMessageBuilderFactory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(SendRequestStep.Factory), typeof(RightRequestResultMessageBuilder.Factory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(AddRequestMessageStep.Factory), typeof(RequestRightsOptionInfoMessageBuilderFactory));

    Container.RegisterSingleton<IRightsRequestOperation, SirenaOperations>();
  }
}