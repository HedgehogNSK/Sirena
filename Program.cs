using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using Telegram.Bot;

namespace Hedgey.Sirena;
static internal class Program
{
  static TelegramBot bot;
  static TelegramBotClient botClient;
  static MongoClient dbClient;
  static FacadeMongoDBRequests request;
  static IMongoDatabase sirenDb;
  public static readonly IMessageSender messageSender;
  static Program()
  {
    var factory = new MongoClientFactory();
    dbClient = factory.Create();
    request = new FacadeMongoDBRequests(dbClient);
    const string dbName = "siren";
    sirenDb = dbClient.GetDatabase(dbName);

    var telegramFactory = new TelegramHelpFactory();
    bot = ((IFactory<TelegramBot>)telegramFactory).Create();
    botClient = ((IFactory<TelegramBotClient>)telegramFactory).Create();
    messageSender = new BotMesssageSender(bot);
    messageSender = new BotMessageSenderTimerProxy(messageSender);
  }
  private static async Task Main(string[] args)
  {

    BotCustomCommmand command = new CreateSirenaCommand("create", "Creates a sirena with certain title", sirenDb, request);
    BotCommands.Add(command);
    command = new CallSirenaCommand("call", "Call sirena by number or by id", request);
    BotCommands.Add(command);
    command = new ListUserSignalsCommand("list", "Shows a list of sirenas that are being tracked.", sirenDb);
    BotCommands.Add(command);
    command = new RemoveSirenCommnad("remove", "Remove your sirena by number, or by id.", sirenDb, request);
    BotCommands.Add(command);
    command = new SubscribeCommand("subscribe", "Subscribes to *sirena* by id.", sirenDb);
    BotCommands.Add(command);
    command = new GetSubscriptionsListCommand("subscriptions", "Displays you current subscriptions.", sirenDb);
    BotCommands.Add(command);
    command = new UnsubscribeCommand("unsubscribe", "Unsubscribes from certain sirena.", sirenDb);
    BotCommands.Add(command);
    command = new GetResponsiblesListCommand("responsible", "Display of people responsible for sirena", sirenDb,request,bot);
    BotCommands.Add(command);
    command = new DelegateRightsCommand("delegate", "Delegate right to call sirena with another user.", sirenDb,request,bot);
    BotCommands.Add(command);
    command = new RevokeRightsCommand("revoke", "Revoke rights to call sirena from user.", sirenDb,request,bot);
    BotCommands.Add(command);
    command = new GetDelegateRequestListCommand("requests", "Display a list of requests for permission to launch a sirena.", sirenDb,request,bot);
    BotCommands.Add(command);

    command = new HelpCommand("help", "Displays list of all commands", bot, BotCommands.Commands);
    BotCommands.Add(command);

    var me = await bot.GetMe();
    // ConcurrentQueue<int?> queue = new ConcurrentQueue<int?>([1, null , 2, 3, 4, 5]);
    // _ = Observable.Timer(TimeSpan.FromSeconds(1))
    //     .Select(_ => queue.TryDequeue(out var result)?result: null)
    //     .Catch((Exception x)=> {
    //       Console.WriteLine(x);
    //       int? f = -1;
    //        return Observable.Return(f);
    //        })
    // .Subscribe(x => Console.WriteLine(x), () => Console.WriteLine(queue.IsEmpty));
    Console.WriteLine($"Bot name: @{me.Username}");

    var subscription = bot.Updates.Message.Subscribe(HandleReceivedMessage, OnError);
    Console.ReadLine();
    subscription.Dispose();
  }

  private static void OnError(Exception exception)
  {
    Console.WriteLine(exception);
  }

  private static void HandleReceivedMessage(Message message)
  {
    if (BotCommands.Contains(message.Text, out BotCustomCommmand? command))
    {
      command?.Execute(message);
    }
    else
    {
      Console.WriteLine($"{message.From.Username}: {message.Text}");
      string text = $"No command were found. Use /help to find out what I can do!";
      messageSender.Send(message.Chat.Id, text);
    }
  }
}