using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using RxTelegram.Bot;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Resources;
using Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class CoreInstaller(Container container) : Installer(container)
{
  const string resourcePath = "Sirena.Resources.Commands";
  public override void Install()
  {
    Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
    
    Container.RegisterSingleton<RequestHandler>();
    Container.RegisterSingleton<ResourceManager>(()=> new ResourceManager(resourcePath, typeof(Program).Assembly));
    Container.RegisterSingleton<ILocalizationProvider, ResourceManagerAdapterLocalizationProvider>();
    Container.RegisterInitializer<ILocalizationProvider>(_provider =>
    {
      MarkupShortcuts.LocalizationProvider = _provider;
    });
    
    Container.Register<IFactory<TelegramBot>,TelegramHelpFactory>();
    Container.Register<IFactory<TelegramBotClient>,TelegramHelpFactory>();
    Container.RegisterSingleton<TelegramBot>(()=> Container.GetInstance<IFactory<TelegramBot>>().Create());//TelegramHelpFactory string token

    Container.RegisterSingleton<IDictionary<long, CommandPlan>,PlanDictionary>();
    Container.RegisterSingleton<PlanScheduler>();
    Container.RegisterSingleton<ActiveCommandsDictionary>();
    Container.RegisterSingleton<IEnumerable<AbstractBotCommmand>>(()=> Container.GetInstance<ActiveCommandsDictionary>().Select(x=>x.Value));
    
    Container.Register<AbstractBotMessageSender,BotMesssageSender>(Lifestyle.Singleton);
    Container.RegisterDecorator<AbstractBotMessageSender,BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageSender,BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageForwarder,BotMessageSenderTimerProxy>(Lifestyle.Singleton);
    Container.Register<IMessageCopier,BotMessageSenderTimerProxy>(Lifestyle.Singleton);
  }

  public class PlanDictionary : Dictionary<long, CommandPlan>
  {
    public PlanDictionary():base()
    {
    }
  }
}