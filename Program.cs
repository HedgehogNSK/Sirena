using Hedgey.Extensions;
using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Callbacks;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Hedgey.Sirena;
static internal class Program
{
  private const string errorNoCommand = $"No known command were found! Use /help to find out what I can do.";
  static TelegramBot bot;
  static MongoClient dbClient;
  static FacadeMongoDBRequests requests;
  public static readonly BotMessageSenderTimerProxy botProxyRequests;
  public static BotCommands botCommands;
  public static readonly PlanScheduler planScheduler;
  private static Dictionary<long, CommandPlan> planDictionary;

  static Program()
  {
    planDictionary = new();
    botCommands = new();
    var factory = new MongoClientFactory();
    dbClient = factory.Create();
    requests = new FacadeMongoDBRequests(dbClient);
    var telegramFactory = new TelegramHelpFactory();
    bot = ((IFactory<TelegramBot>)telegramFactory).Create();
    var botMesssageSender = new BotMesssageSender(bot);
    botProxyRequests = new BotMessageSenderTimerProxy(botMesssageSender, botMesssageSender, botMesssageSender);
    planScheduler = new PlanScheduler();
  }
  private static void Initialization()
  {
    var commandFactory = new CommandFactory(requests, bot, botCommands
    , planScheduler, botProxyRequests, botProxyRequests, botProxyRequests);
    var botCommandsInitializer = new CommandsCollectionInitializer(commandFactory);
    botCommandsInitializer.Initialize(botCommands);//Fill bot commands collection only with working commands
  }
  private static async Task Main(string[] args)
  {
    Initialization();

    var me = await bot.GetMe();
    Console.WriteLine($"Bot name: @{me.Username}");
    var observableMessages = bot.Updates.Message
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Updates.Messages exception: {0}", _ex);
          OnError(_ex);
          return Observable.Empty<Message>().Delay(TimeSpan.FromSeconds(1));
        })
        .Repeat()
        .Select(_message => (IRequestContext)new MessageRequestContext(_message));

    var observableCallbackPublisher = bot.Updates.CallbackQuery
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Updates.CallbackQuery exception: {0}", _ex);

          OnError(_ex);
          return Observable.Empty<CallbackQuery>().Delay(TimeSpan.FromSeconds(1));
        })
        .Repeat()
        .Publish();

    var constexStream = observableCallbackPublisher
        .Select(_query => new CallbackRequestContext(_query))
        .Merge(observableMessages)
        .Subscribe(DetermineAndExecuteCommand, OnError);

    var approveCallbackStream = observableCallbackPublisher
        .SelectMany(SendCallbackApprove)
        .Catch((Exception _ex) =>
        {
          Console.WriteLine("Send callback approve exception: {0}", _ex);

          OnError(_ex);
          return Observable.Empty<bool>();
        })
        .Repeat()
        .Subscribe(_ => { }, OnError);

    var callbackStream = observableCallbackPublisher.Connect();
    var schedulerTrackPublisher = planScheduler.Track()
        .Catch((Exception _ex) =>
        {
          OnError(_ex);
          return Observable.Empty<CommandPlan.Report>();
        })
        .Repeat()
        .Publish();
    var planProcessingStream = schedulerTrackPublisher
        .Subscribe(ProcessPlanReport);
#pragma warning disable CS8604 // Possible null reference argument.
    var sendMessagesStream = schedulerTrackPublisher
        .Where(_report => _report.StepReport.MessageBuilder != null)
        .SelectMany(_report => botProxyRequests.ObservableSend(_report.StepReport.MessageBuilder))
        .Subscribe();
#pragma warning restore CS8604 // Possible null reference argument.
    var schedulerTrackStream = schedulerTrackPublisher.Connect();

    IDisposable subscription = new CompositeDisposable(callbackStream
    , constexStream, approveCallbackStream, planProcessingStream
    , sendMessagesStream, schedulerTrackStream);
    string? input;
    do
    {
      input = Console.ReadLine();
    } while (input != "exit");
    planScheduler.Dispose();
    planProcessingStream.Dispose();
    subscription.Dispose();
  }

  private static void ProcessPlanReport(CommandPlan.Report report)
  {
    var uid = report.Plan.Context.GetUser().Id;
    switch (report.StepReport.Result)
    {
      case CommandStep.Result.Success:
        {
          if (report.Plan.IsComplete)
            planDictionary.Remove(uid);
        }; break;
      case CommandStep.Result.Canceled:
      case CommandStep.Result.Exception: planDictionary.Remove(uid); break;
      case CommandStep.Result.Wait: planDictionary[uid] = report.Plan; break;
      default:
        throw new ArgumentOutOfRangeException(nameof(report.StepReport.Result));
    }
  }

  private static void OnError(Exception exception)
  {
    var time = Shortucts.CurrentTimeLabel();
    var ex = exception;
    do
    {
      switch (ex)
      {
        case ApiException apiException:
          {
            string message = time + apiException.Message + ": " + apiException.Description;
            Console.WriteLine(message);
          }
          break;
        default: Console.WriteLine(time + exception.Message); break;
      }
      ex = ex.InnerException;
    }
    while (ex != null);
  }
  private static IObservable<bool> SendCallbackApprove(CallbackQuery query)
  {
    var callbackAnswer = new AnswerCallbackQuery()
    {
      CallbackQueryId = query.Id,
    };

    return Observable.FromAsync(() => bot.AnswerCallbackQuery(callbackAnswer))
      .Catch((ApiException _ex) =>
    {
      throw new Exception($"Error on callback answer to user {query.From.Id} on request: \"{query.Data}\"", _ex);
    });
  }
  private static void DetermineAndExecuteCommand(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    bool commandIsSet = botCommands.TryGetCommand(context.GetCommandName(), out var command);
    bool planIsSet = planDictionary.TryGetValue(uid, out CommandPlan? plan);

    if (!commandIsSet && !planIsSet)
    {
      botProxyRequests.Send(uid, errorNoCommand);
      return;
    }

    if (!commandIsSet)
    {
      ExecutePlan();
      return;
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    if (planIsSet)
    {
      if (command.Command.Equals(plan.contextContainer.Object.GetCommandName()))
      {
        ExecutePlan();
        return;
      }
      else
        planDictionary.Remove(uid);
    }

    try
    {
      var time = Shortucts.CurrentTimeLabel();
      Console.WriteLine($"{time}{uid} -> {command.Command}");
      command.Execute(context);
    }
    catch (Exception ex)
    {
      OnError(ex);
    }
    void ExecutePlan()
    {
      string name = plan.contextContainer.Object.GetCommandName();
      Console.WriteLine($"{uid}: update -> {name}");
      plan.Update(context);
      planScheduler.Push(plan);
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
  }
}