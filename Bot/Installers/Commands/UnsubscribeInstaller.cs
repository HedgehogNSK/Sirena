using Hedgey.Extensions.SimpleInjector;
using Hedgey.Localization;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot.DI;

public class UnsubscribeInstaller(SimpleInjector.Container container)
 : PlanBassedCommandInstaller<UnsubscribeCommand, UnsubscribeSirenaPlanFactory>(container)
{
  public override void Install()
  {
    base.Install();

    Container.RegisterStepFactoryWithBuilderFactory(typeof(ProcessParameterUnsubscribeStep.Factory)
    , typeof(SubscriptionsMessageBuilderFactory));
    Container.RegisterStepFactoryWithBuilderFactory(typeof(TryUnsubscribeStep.Factory)
    , typeof(UnsubscribeMessageBuilder.Factory));

    var switchButtonEditReplyMarkupRegistration = lifestyle.CreateRegistration(() =>
    {
      var provider = Container.GetInstance<ILocalizationProvider>();
      return new SwitchButtonCommandReplyMarkupBuilder.Factory(provider
        , new SwitchButtonCommandReplyMarkupBuilder.Option(SubscribeCommand.NAME, MarkupShortcuts.subscribeTitle)
      );
    }, Container);
    Container.RegisterConditional<IFactory<IRequestContext, IEditMessageReplyMarkupBuilder>>(
      switchButtonEditReplyMarkupRegistration
      , (context) => context.Consumer.ImplementationType == typeof(TryUnsubscribeStep.Factory)
    );
  }
}