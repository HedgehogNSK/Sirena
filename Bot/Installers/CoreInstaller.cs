using Hedgey.Extensions.Blendflake;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Localization;
using Hedgey.Sirena.ID;
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
  //DO NOT TOUCH TIMESTAMP OR NEW ID COULD OVERLAP EXISTING IDs
  const long EPOCH_TIMESTAMP = 1736959000000;  //Wed Jan 15 2025 16:36:40 GMT+0000

  public override void Install()
  {
    Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

    Container.RegisterSingleton<RequestHandler>();
    Container.RegisterSingleton<ResourceManager>(() => new ResourceManager(resourcePath, typeof(Program).Assembly));
    Container.RegisterSingleton<ILocalizationProvider, ResourceManagerAdapterLocalizationProvider>();
    Container.RegisterInitializer<ILocalizationProvider>(_provider 
      => MarkupShortcuts.LocalizationProvider = _provider);

    Container.Register<IFactory<TelegramBot>, TelegramHelpFactory>();
    Container.Register<IFactory<TelegramBotClient>, TelegramHelpFactory>();
    Container.RegisterSingleton<TelegramBot>(() => Container.GetInstance<IFactory<TelegramBot>>().Create());

    Container.RegisterSingleton<IDictionary<long, CommandPlan>, PlanDictionary>();
    Container.RegisterSingleton<PlanScheduler>();
    Container.RegisterSingleton<ActiveCommandsDictionary>();
    Container.RegisterSingleton<IEnumerable<AbstractBotCommmand>>(() => Container.GetInstance<ActiveCommandsDictionary>().Select(x => x.Value));

    Container.Register<AbstractBotMessageSender, BotMesssageSender>(Lifestyle.Singleton);
    Container.RegisterDecorator<AbstractBotMessageSender, BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageSender, BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageForwarder, BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageCopier, BotMessageSenderTimerProxy>(Lifestyle.Singleton);

    Container.RegisterSingleton<IIDGenerator>(() => new BlendflakeAdapter(EPOCH_TIMESTAMP, MACHINE_ID));
  }

  public class PlanDictionary : Dictionary<long, CommandPlan>
  {
    public PlanDictionary() : base()
    {
    }
  }
}