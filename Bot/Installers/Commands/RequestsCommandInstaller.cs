using Hedgey.Extensions.SimpleInjector;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class RequestsCommandInstaller(Container container)
   : CommandInstaller<RequestsCommand>(container)
{
  public override void Install()
  {
    base.Install();

    RegisterIntoCommand<IFactory<IRequestContext, SirenasListMessageBuilder>
    , SirenasRequestListsMessageBuilderFactory>(Lifestyle.Singleton);

    Container.RegisterStepFactoryWithBuilderFactories(typeof(GetUserSirenasStep.Factory)
    , [typeof(NoRequestsMessageBuilderFactory), typeof(SirenaRequestsSendMessageBuilder.Factory)]);

    RegisterIntoCommand<IFactory<NullableContainer<IEnumerable<SirenaData>>, LoadUserSirenasWithRequestsStep>
    , LoadUserSirenasWithRequestsStep.Factory>(Lifestyle.Singleton);

    RegisterIntoCommand<IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep>
    , RequestsValidateSirenaIdStep.Factory>(Lifestyle.Singleton);

    Container.RegisterStepFactoryWithBuilderFactory(typeof(GetUserSirenaStep.Factory)
    , typeof(SirenaNotFoundMessageBuilder.Factory));

    Container.RegisterStepFactoryWithBuilderFactories(typeof(DisplaySirenaRequestsStep.Factory)
    , [typeof(SirenaRequestsSendMessageBuilder.Factory), typeof(SirenaRequestsEditMessageBuilder.Factory)
    , typeof(SirenaHasNoRequestsMessageBuilderFactory)]);

    RegisterIntoCommand<IFactory<NullableContainer<SirenaData>
      , NullableContainer<RequestsCommand.RequestInfo>, CreateRequestInfoStep>
      , CreateRequestInfoStep.Factory>(Lifestyle.Singleton);
  }
}