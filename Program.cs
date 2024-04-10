using Hedgey.Extensions;
using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Callbacks;
using System.Reactive.Linq;

namespace Hedgey.Sirena;
static internal class Program
{
  private const string errorWrongFormat = "You have to send command. Commands starts from '/'. Use /help to find out options.";
  private const string errorNoCommand = $"No command were found. Use /help to find out what I can do!";
  static TelegramBot bot;
  static MongoClient dbClient;
  static FacadeMongoDBRequests request;
  public static readonly IMessageSender messageSender;
  public static readonly BotCommands botCommands;

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
    var commandsCollectionFactory = new CommandsCollectionFactory(request, bot);
    botCommands = commandsCollectionFactory.Create();
  }
  private static async Task Main(string[] args)
  {
    var me = await bot.GetMe();
    Console.WriteLine($"Bot name: @{me.Username}");

    var subscription = bot.Updates.Message.Subscribe(HandleReceivedMessage, OnError);
    bot.Updates.CallbackQuery.Subscribe(OnInlineCallback, OnError);

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

  private static async void OnInlineCallback(CallbackQuery query)
  {
    var callbackAnswer = new AnswerCallbackQuery(){
       CallbackQueryId = query.Id,
        Text = "ALLO",
         
    };
    try{
    bool result = await bot.AnswerCallbackQuery(callbackAnswer);
    }
    catch (ApiException ex)
    {
      var wrappedEx = new Exception($"Exception in {query.From.Id} on command: {query.Data}\nreason: {ex.StatusCode}\n{ex.Description}", ex);
      Console.WriteLine(wrappedEx);

      return;
    }
    string argString;
    AbstractBotCommmand? command;
    if (!GetCommand(query.Data, query.From.Id, out argString, out command) || command==null)
      return;
    var context = new CommandContext(query.From, query.Message.Chat, command.Command, argString);
    command?.Execute(context);
  }

  private static void HandleReceivedMessage(Message message)
  {
    string argString;
    AbstractBotCommmand? command;
    if (!GetCommand(message.Text, message.From.Id, out argString, out command) || command==null)
      return;
    var context = new CommandContext(message.From, message.Chat, command.Command, argString);
    Console.WriteLine($"user {context.GetUser().Id} calls {context.GetCommandName()}");
    command?.Execute(context);
  }

  private static bool GetCommand(string source, long senderId, out string argString, out AbstractBotCommmand? command)
  {
    command = null;
    if (!TextTools.ExtractCommandAndArgs(source, out string commandName, out argString))
    {
      messageSender.Send(senderId, errorWrongFormat);
      return false;
    }
    command = botCommands.GetCommmandOrNull(commandName);
    if (command == null)
    {
      messageSender.Send(senderId, errorNoCommand);
      return false;
    }
    return true;
  }
}