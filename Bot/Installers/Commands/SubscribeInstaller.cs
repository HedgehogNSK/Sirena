using Hedgey.Extensions.SimpleInjector;
using Hedgey.Localization;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using SimpleInjector;

namespace Hedgey.Sirena.Bot.DI;

public class SubscribeInstaller(Container container)
 : PlanBassedCommandInstaller<SubscribeCommand, SubscribeSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.Register<AskSirenaIdMessageBuilder.Factory>();
    var creationFunction = () => new ValidateSirenaIdStep.Factory(Container.GetInstance<AskSirenaIdMessageBuilder.Factory>());
    RegisterIntoPlanFactory<IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>>
    (Lifestyle.Singleton, creationFunction);

    Container.RegisterStepFactoryWithBuilderFactories(typeof(RequestSubscribeStep.Factory)
    , [typeof(SuccesfulSubscriptionMessageBuilder.Factory), typeof(SirenaNotFoundMessageBuilder.Factory)]);

    var switchButtonEditReplyMarkupRegistration = lifestyle.CreateRegistration(() =>
    {
      var provider = Container.GetInstance<ILocalizationProvider>();
      return new SwitchButtonCommandReplyMarkupBuilder.Factory(provider, new SwitchButtonCommandReplyMarkupBuilder.Option(UnsubscribeCommand.NAME, MarkupShortcuts.unsubscribeTitle));
    }, Container);
    Container.RegisterConditional<IFactory<IRequestContext, IEditMessageReplyMarkupBuilder>>(
      switchButtonEditReplyMarkupRegistration
      , (context) => context.Consumer.ImplementationType == typeof(RequestSubscribeStep.Factory)
    );
  }
}