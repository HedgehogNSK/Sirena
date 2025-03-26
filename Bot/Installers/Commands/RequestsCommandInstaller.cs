using Hedgey.Extensions.SimpleInjector;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class RequestsCommandInstaller(Container container)
   : PlanBassedCommandInstaller<RequestsCommand, ReuquestsPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    RegisterIntoPlanFactory<IFactory<IRequestContext, SirenasListMessageBuilder>
    , SirenasRequestListsMessageBuilderFactory>(Lifestyle.Singleton);

    Container.RegisterStepFactoryWithBuilderFactory(typeof(GetUserSirenasStep.Factory)
    , typeof(NoRequestsMessageBuilderFactory));

    RegisterIntoPlanFactory<IFactory<NullableContainer<ulong>, ISendMessageBuilder, ValidateSirenaIdStep2>
    , ValidateSirenaIdStep2.Factory>(Lifestyle.Singleton);

    Container.RegisterStepFactoryWithBuilderFactory(typeof(GetUserSirenaStep.Factory)
    , typeof(SirenaNotFoundMessageBuilder.Factory));

    Container.RegisterStepFactoryWithBuilderFactories(typeof(DisplaySirenaRequestsStep.Factory)
    , [typeof(SirenaRequestsSendMessageBuilder.Factory), typeof(SirenaRequestsEditMessageBuilder.Factory)
    , typeof(SirenaHasNoRequestsMessageBuilderFactory)]);
  }
}