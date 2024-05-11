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
using System.Reactive.Threading.Tasks;

namespace Hedgey.Sirena;
static internal class Program
{
  private const string errorNoCommand = $"No known command were found! Use /help to find out what I can do.";
  static TelegramBot bot;
  static MongoClient dbClient;
  static FacadeMongoDBRequests request;
  public static readonly IMessageSender messageSender;
  public static BotCommands botCommands;
  public static readonly PlanScheduler planScheduler;
  static Dictionary<long, CommandPlan> planDictionary;
  static Program()
  {
    planDictionary = new();
    var factory = new MongoClientFactory();
    dbClient = factory.Create();
    request = new FacadeMongoDBRequests(dbClient);
    var telegramFactory = new TelegramHelpFactory();
    bot = ((IFactory<TelegramBot>)telegramFactory).Create();
    messageSender = new BotMesssageSender(bot);
    messageSender = new BotMessageSenderTimerProxy(messageSender);
    planScheduler = new PlanScheduler();
  }
  private static async Task Main(string[] args)
  {
    var commandsCollectionFactory = new CommandsCollectionFactory(request, bot, planScheduler);
    botCommands = commandsCollectionFactory.Create();
    var me = await bot.GetMe();
    Console.WriteLine($"Bot name: @{me.Username}");
    var observableMessages = bot.Updates.Message
        .Select(_message => (IRequestContext)new MessageRequestContext(_message));

    var observableCallbackPublisher = bot.Updates.CallbackQuery.Publish();
    var observableCallbackContext = observableCallbackPublisher
        .Select(_query => new CallbackRequestContext(_query));

    var constexStream = observableMessages.Merge(observableCallbackContext)
        .Subscribe(DetermineAndExecuteCommand, OnError);

    var approveCallbackStream = observableCallbackPublisher
        .SelectMany(SendCallbackApprove)
        .Subscribe(Console.WriteLine, OnError);

    var callbackStream = observableCallbackPublisher.Connect();
    var schedulerTrackPublisher = planScheduler.Track().Publish();
    var planProcessingStream = schedulerTrackPublisher.Subscribe(ProcessPlanSummary);
    var sendMessagesStream = schedulerTrackPublisher.Subscribe(SendResult);
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

  private static void SendResult(CommandPlan.Report report)
  {
    var message = report.LastStepReport?.MessageBuilder?.Build() ?? null;
    if (message == null)
      return;
    messageSender.Send(message);
  }

  private static void ProcessPlanSummary(CommandPlan.Report report)
  {
    var uid = report.Plan.contextContainer.Object.GetUser().Id;
    switch (report.LastStepReport.Result)
    {
      case CommandStep.Result.Success:
      case CommandStep.Result.Canceled:
      case CommandStep.Result.Exception:
        {

          planDictionary.Remove(uid);
        }
        break;
      case CommandStep.Result.Wait:
        {
          planDictionary[uid] = report.Plan;
        }
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(report.LastStepReport.Result)
      , "Exception caught for plan:" + report.Plan.GetType().Name + " on step: " + report.LastStepReport.GetType().Name);
    }
  }

  private static void OnError(Exception exception)
  {
    Console.WriteLine("Callback Exception! " + exception);
  }
  private static IObservable<bool> SendCallbackApprove(CallbackQuery query)
  {
    var callbackAnswer = new AnswerCallbackQuery()
    {
      CallbackQueryId = query.Id,
    };

    return bot.AnswerCallbackQuery(callbackAnswer).ToObservable().Catch((ApiException ex) =>
    {
      var wrappedEx = new Exception($"Exception in {query.From.Id} on command: {query.Data}\nreason: {ex.StatusCode}\n{ex.Description}", ex);
      Console.WriteLine(wrappedEx);
      throw wrappedEx;
    });
  }
  private static void DetermineAndExecuteCommand(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    AbstractBotCommmand? command = GetCommmand(context);
    if (command == null)
    {

      if (planDictionary.TryGetValue(uid, out CommandPlan plan))
      {
        plan.contextContainer.Set(context);
        planScheduler.Push(plan);
        return;
      }
      else
      {
        messageSender.Send(uid, errorNoCommand);
        return;
      }
    }
    planDictionary.Remove(uid);
    Console.WriteLine($"user {uid} calls {context.GetCommandName()}");
    command.Execute(context);
  }
  private static AbstractBotCommmand? GetCommmand(IRequestContext context)
  {
    if (string.IsNullOrEmpty(context.GetCommandName()))
      return null;
    var command = botCommands.Find(context.IsValid);
    return command;
  }
}