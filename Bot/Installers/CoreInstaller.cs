using Hedgey.Extensions;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Resources;
using Telegram.Bot;

namespace Hedgey.Sirena.Bot.DI;

public class CoreInstaller(Container container) : Installer(container)
{
  const string resourcePath = "Sirena.Resources.Commands";
  const int MACHINE_ID = 0;
  const long EPOCH_TIMESTAMP = 1725182332000; //2024-08-29T10:00:00.000Z

  public override void Install()
  {
    Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

    Container.RegisterSingleton<RequestHandler>();
    Container.RegisterSingleton<ResourceManager>(() => new ResourceManager(resourcePath, typeof(Program).Assembly));
    Container.RegisterSingleton<ILocalizationProvider, ResourceManagerAdapterLocalizationProvider>();
    Container.RegisterInitializer<ILocalizationProvider>(_provider =>
    {
      MarkupShortcuts.LocalizationProvider = _provider;
    });

    Container.Register<IFactory<TelegramBot>, TelegramHelpFactory>();
    Container.Register<IFactory<TelegramBotClient>, TelegramHelpFactory>();
    Container.RegisterSingleton<TelegramBot>(() => Container.GetInstance<IFactory<TelegramBot>>().Create());//TelegramHelpFactory string token

    Container.RegisterSingleton<IDictionary<long, CommandPlan>, PlanDictionary>();
    Container.RegisterSingleton<PlanScheduler>();
    Container.RegisterSingleton<ActiveCommandsDictionary>();
    Container.RegisterSingleton<IEnumerable<AbstractBotCommmand>>(() => Container.GetInstance<ActiveCommandsDictionary>().Select(x => x.Value));

    Container.Register<AbstractBotMessageSender, BotMesssageSender>(Lifestyle.Singleton);
    Container.RegisterDecorator<AbstractBotMessageSender, BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageSender, BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageForwarder, BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageCopier, BotMessageSenderTimerProxy>(Lifestyle.Singleton);

    Container.RegisterSingleton<IIDGenerator>(()=> new CustomSnowflakeGenerator(EPOCH_TIMESTAMP, MACHINE_ID));
  }

  public class PlanDictionary : Dictionary<long, CommandPlan>
  {
    public PlanDictionary() : base()
    {
    }
  }
}