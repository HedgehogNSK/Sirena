using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;

namespace Hedgey.Sirena;
static internal class Program
{
  static TelegramBot bot;
  static MongoClient dbClient;
  static FacadeMongoDBRequests request;
  public static readonly IMessageSender messageSender;
  static Program()
  {
    var factory = new MongoClientFactory();
    dbClient = factory.Create();
    request = new FacadeMongoDBRequests(dbClient);
    var telegramFactory = new TelegramHelpFactory();
    bot = ((IFactory<TelegramBot>)telegramFactory).Create();
    //TelegramBotClient botClient = ((IFactory<TelegramBotClient>)telegramFactory).Create();
    messageSender = new BotMesssageSender(bot);
    messageSender = new BotMessageSenderTimerProxy(messageSender);
  }
  private static async Task Main(string[] args)
  {

    BotCustomCommmand command = new CreateSirenaCommand("create", "Creates a sirena with certain title. Example: `/create Sirena`", request.db, request);
    BotCommands.Add(command);
    command = new CallSirenaCommand("call", "Call sirena by number or by id", request);
    BotCommands.Add(command);
    command = new ListUserSignalsCommand("list", "Shows a list of sirenas that are being tracked.", request.db);
    BotCommands.Add(command);
    command = new RemoveSirenCommnad("remove", "Remove your sirena by number, or by id.", request.db, request);
    BotCommands.Add(command);
    command = new SubscribeCommand("subscribe", "Subscribes to *sirena* by id.", request.db);
    BotCommands.Add(command);
    command = new GetSubscriptionsListCommand("subscriptions", "Displays you current subscriptions.", request.db);
    BotCommands.Add(command);
    command = new UnsubscribeCommand("unsubscribe", "Unsubscribes from certain sirena.", request.db);
    BotCommands.Add(command);
    command = new MuteUserSignalCommand("mute", "Mute calls from certain user for certain *sirena*. Calls of the *sirena* from other users will be active anyway",request,bot);
    BotCommands.Add(command);
    command = new UnmuteUserSignalCommand("unmute", "Unmute previously muted user for certain siren",request,bot);
    BotCommands.Add(command);
    command = new GetResponsiblesListCommand("responsible", "Display of people responsible for sirena", request.db,request,bot);
    BotCommands.Add(command);
    command = new DelegateRightsCommand("delegate", "Delegate right to call sirena with another user.", request.db,request,bot);
    BotCommands.Add(command);
    command = new RevokeRightsCommand("revoke", "Revoke rights to call sirena from user.",request,bot);
    BotCommands.Add(command);
    command = new RequestRightsCommand("request", "Allows to request rights to call certain sirena of another user.", request);
    BotCommands.Add(command);
    command = new GetRequestsListCommand("requests", "Display a list of requests for permission to launch a sirena.", request.db,request,bot);
    BotCommands.Add(command);

    command = new HelpCommand("help", "Displays list of all commands", bot, BotCommands.Commands);
    BotCommands.Add(command);

    command = new StartCommand("start", "Initialization of user", request);
    BotCommands.Add(command);

    var me = await bot.GetMe();
    Console.WriteLine($"Bot name: @{me.Username}");

    var subscription = bot.Updates.Message.Subscribe(HandleReceivedMessage, OnError);

    string? input;
    do
    {
      input = Console.ReadLine();
    } while (input != "exit");
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