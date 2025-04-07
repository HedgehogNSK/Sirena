using Hedgey.Extensions.NetCoreServer;
using Hedgey.Extensions.SimpleInjector;
using Hedgey.Extensions.Telegram;
using Hedgey.Extensions.Types;
using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Bot.DI;
using Hedgey.Sirena.Bot.DI.HTTP;
using Hedgey.Sirena.HTTP.Server;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Api;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Callbacks;
using RxTelegram.Bot.Interface.Setup;
using SimpleInjector;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena;
static internal class Program
{
  private static async Task Main(string[] args)
  {
    Container container = new Container();
    Installer installer = new CoreInstaller(container);
    installer.Install();
    installer = new ServerInstaller(container);
    installer.Install();
    installer = new SharedCommandServicesInstaller(container);
    installer.Install();
    InstallCommands(container);

    container.Verify();

    var botProxyRequests = container.GetInstance<AbstractBotMessageSender>();
    var bot = container.GetInstance<TelegramBot>();
    var me = await bot.GetMe();
    Console.WriteLine($"Bot name: @{me.Username}");

    //HTTP(s) server initialization
    var server = container.GetInstance<HttpsServer>();
    StartServer(server);

    SetWebhook setWebhook = container.GetInstance<SetWebhook>();
    try
    {
      await bot.SetWebhook(setWebhook);
      await bot.DisplayWebhookInfo();
    }
    catch (Exception ex)
    {
      ExceptionHandler.OnError(ex);
      if (server.IsStarted)
        server.Stop();
      await SwitchToLongpolling();
    }

    var observableMessages = bot.Updates.Message
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Exception on observing Update.Message");
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<Message>().Delay(TimeSpan.FromSeconds(1));
        })
        .Repeat()
        .Select(_message => (IRequestContext)new MessageRequestContext(_message));

    var observableCallbackPublisher = bot.Updates.CallbackQuery
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Exception on observing Updates.CallbackQuery");
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<CallbackQuery>().Delay(TimeSpan.FromSeconds(1));
        })
        .Repeat()
        .Publish();

    var requestHandler = container.GetInstance<RequestHandler>();
    var constexStream = observableCallbackPublisher
        .Select(_query => new CallbackRequestContext(_query))
        .Merge(observableMessages)
        .Subscribe(requestHandler.Process, ExceptionHandler.OnError);

    var approveCallbackStream = observableCallbackPublisher
        .SelectMany(SendCallbackApprove)
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Exception on sending Callback Approve");
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<bool>();
        })
        .Repeat()
        .Subscribe();

    var callbackStream = observableCallbackPublisher.Connect();

    //Plan scheduler subscriptions
    var planScheduler = container.GetInstance<PlanScheduler>();
    var schedulerTrackPublisher = planScheduler.Track()
        .Catch((Exception _ex) =>
        {
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<CommandPlan.Report>();
        })
        .Repeat()
        .Publish();

    var planProcessingStream = schedulerTrackPublisher
        .Subscribe(requestHandler.ProcessPlanReport);

#pragma warning disable CS8604 // Possible null reference argument.
    var sendMessagesStream = schedulerTrackPublisher
        .Where(_report => _report.StepReport.MessageBuilder != null)
        .SelectMany(_report => botProxyRequests.ObservableSend(_report.StepReport.MessageBuilder))
        .Catch((Exception _ex) =>
        {
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<Message>();
        })
        .Repeat()
        .Subscribe();

    var editMessagesStream = schedulerTrackPublisher
        .Where(_report => _report.StepReport.EditMessageBuilder != null)
        .SelectMany(_report => botProxyRequests.Edit(_report.StepReport.EditMessageBuilder))
        .Catch((Exception _ex) =>
        {
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<Message>();
        })
        .Repeat()
        .Subscribe();

    var editReplyMarkupStream = schedulerTrackPublisher
        .Where(_report => _report.StepReport.EditMessageReplyMarkupBuilder != null)
        .SelectMany(_report => botProxyRequests.Edit(_report.StepReport.EditMessageReplyMarkupBuilder))
        .Catch((Exception _ex) =>
        {
          ExceptionHandler.OnError(_ex);
          return Observable.Empty<Message>();
        })
        .Repeat()
        .Subscribe();


    var fallbackStream = schedulerTrackPublisher
        .Where(_report => _report.StepReport.Fallback != null)
        .Subscribe(_report => requestHandler.Process(_report.StepReport.Fallback), ExceptionHandler.OnError);
#pragma warning restore CS8604 // Possible null reference argument.

    var schedulerTrackStream = schedulerTrackPublisher.Connect();

    IDisposable subscription = new CompositeDisposable(callbackStream
    , constexStream, approveCallbackStream, planProcessingStream
    , sendMessagesStream, schedulerTrackStream, editMessagesStream
    , editReplyMarkupStream, fallbackStream);

    string? input;
    do
    {
      input = Console.ReadLine();
      switch (input)
      {
        case "set wh": { await SwitchToWebHook(); } break;
        case "set lp": { await SwitchToLongpolling(); } break;
        default: break;
      }
    } while (input != "exit");

    planScheduler.Dispose();
    planProcessingStream.Dispose();
    subscription.Dispose();
    server.Stop();
    await bot.DeleteWebhook();

    IObservable<bool> SendCallbackApprove(CallbackQuery query)
    {
      var callbackAnswer = new AnswerCallbackQuery()
      {
        CallbackQueryId = query.Id,
      };

      return Observable.FromAsync(() => bot.AnswerCallbackQuery(callbackAnswer))
        .Catch((ApiException _ex) =>
          throw new InvalidOperationException($"Error on callback answer to user {query.From.Id} on request: \"{query.Data}\"", _ex)
        );
    }

    async Task SwitchToLongpolling()
    {
      Console.WriteLine("Switching to Longpolling requests");
      await bot.DeleteWebhook();
      if (server.IsStarted)
        server.Stop();
      var tracker = new LongPollingUpdateTracker(bot);
      bot.Updates.Set(tracker);
    }

    async Task SwitchToWebHook()
    {
      StartServer(server);
      var updateHandler = container.GetInstance<UpdateHandler>();
      bot.Updates.Set(updateHandler.Update);
      await bot.SetWebhook(setWebhook);
      await bot.DisplayWebhookInfo();
    }
  }

  private static void StartServer(HttpsServer server)
  {
    if (server.IsStarted)
    {
      Console.WriteLine("Server is already launched");
      return;
    }
    bool httpStarted = server.Start();
    Console.WriteLine($"Starting HTTP server on port {server.Port}...");
    if (!httpStarted)
      throw new ActivationException("Server is not started");
  }

  private static void InstallCommands(Container container)
  {
    var installerTypes = System.Reflection.Assembly.GetExecutingAssembly()
      .GetTypes()
      .Where(_type => !_type.IsGenericType && _type.HasParent(typeof(CommandInstaller<>)));

    foreach (var type in installerTypes)
    {
      var instance = Activator.CreateInstance(type, container) as Installer;
      instance?.Install();
    }
  }
}