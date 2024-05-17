using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot;

namespace Hedgey.Sirena.Bot;

public class CommandsCollectionFactory : IFactory<BotCommands>
{
  private readonly FacadeMongoDBRequests requests;
  private readonly TelegramBot bot;
  private readonly PlanScheduler planScheduler;
  private readonly IMessageSender messageSender;

  public CommandsCollectionFactory(FacadeMongoDBRequests requests
    , TelegramBot bot, PlanScheduler planScheduler, IMessageSender messageSender)
  {
    this.requests = requests;
    this.bot = bot;
    this.planScheduler = planScheduler;
    this.messageSender = messageSender;
  }

  public BotCommands Create()
  {

    BotCommands botCommands = new();
    var botCommandsFactory = new CommandFactory(requests, bot, botCommands, planScheduler,messageSender);
    var botCommandsInitializer = new CommandsCollectionInitializer(botCommands, botCommandsFactory);
    botCommandsInitializer.Initialize();
    return botCommands;
  }
}