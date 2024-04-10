using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot;

namespace Hedgey.Sirena.Bot;

public class CommandsCollectionFactory : IFactory<BotCommands>{
  private readonly FacadeMongoDBRequests requests;
  private readonly TelegramBot bot;

  public CommandsCollectionFactory(FacadeMongoDBRequests requests, TelegramBot bot)
  {
    this.requests = requests;
    this.bot = bot;
  }

  public BotCommands Create(){
    
    BotCommands botCommands = new();
    var botCommandsFactory = new CommandFactory(requests, bot, botCommands);
    var botCommandsInitializer = new CommandsCollectionInitializer(botCommands,botCommandsFactory);
    botCommandsInitializer.Initialize();
    return botCommands;
  }
}
