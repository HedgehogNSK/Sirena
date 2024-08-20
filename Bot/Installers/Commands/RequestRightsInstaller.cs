using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot.DI;

public class RequestRightsInstaller(SimpleInjector.Container container)
   : PlanBassedCommandInstaller<RequestRightsCommand, RequestRightsPlanFactory>(container)
  {
    public override void Install()
    {
      base.Install();

      Container.RegisterConditional<IFactory<IRequestContext, IRightsRequestOperation.Result, IMessageBuilder>, RightRequestResultMessageBuilder.Factory>(
        _context => _context.Consumer.ImplementationType == typeof(SendRequestStep)
      );
    }
  }
