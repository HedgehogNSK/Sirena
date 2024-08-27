using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;

namespace Hedgey.Sirena.Bot.DI;

public class RequestRightsInstaller(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<RequestRightsCommand, RequestRightsPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterStepFactoryWithBuilderFactory(typeof(DisplayCommandMenuStep.Factory), typeof(RequestListMessageBuilderFactory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(SendRequestStep.Factory), typeof(RightRequestResultMessageBuilder.Factory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(AddRequestMessageStep.Factory), typeof(RequestRightsOptionInfoMessageBuilderFactory));

    Container.RegisterSingleton<IRightsRequestOperation, SirenaOperations>();
  }
}