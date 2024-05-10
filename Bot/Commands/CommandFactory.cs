using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Bot.Operations.Mongo;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using RxTelegram.Bot;

namespace Hedgey.Sirena.Bot;

public class CommandFactory : IFactory<string, AbstractBotCommmand>
{
  private readonly IMongoDatabase db;
  private readonly FacadeMongoDBRequests requests;
  private readonly TelegramBot bot;
  private readonly BotCommands botCommands;
  private readonly PlanScheduler planScheduler;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;
private readonly IFindSirenaOperation findSirenaOperation;
  public CommandFactory(FacadeMongoDBRequests requests, TelegramBot bot, BotCommands botCommands, PlanScheduler planScheduler)
  {
    this.requests = requests;
    this.db = requests.db;
    this.bot = bot;
    this.botCommands = botCommands;
    this.planScheduler = planScheduler;
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    users = db.GetCollection<UserRepresentation>("users");
    findSirenaOperation = new FindSirenaOperation(sirens);
  }
  public AbstractBotCommmand Create(string commandName)
  {
    switch (commandName)
    {
      case "menu":
        {
          IGetUserOverviewAsync getUserOverview = new GetUserStatsOperationAsync(sirens);
          return new MenuBotCommand(getUserOverview);
        }
      case "call": return new CallSirenaCommand(requests);
      case "create":
        {
          var getUserStats = new GetUserOperationAsync(users, requests);
          var createSiren = new CreateSirenaOperationAsync(sirens, users);
          var factory = new CreateSirenaPlanFactory(getUserStats, createSiren);
          return new CreateSirenaCommand(factory, planScheduler);
        }
      case "delegate": return new DelegateRightsCommand(requests.db, requests, bot);
      case "help": return new HelpCommand(botCommands.Commands);
      case "list": return new ListUserSignalsCommand(requests.db);
      case "mute": return new MuteUserSignalCommand(requests, bot);
      case DeleteSirenaCommand.NAME:
        {
          var findUsersSirenaOperation = new FindUsersSirenasOperation(sirens);
          var deleteSirenaOperation = new DeleteSirenaOperation(sirens, users);
          var factory = new DeleteSirenaPlanFactory(findSirenaOperation, findUsersSirenaOperation, deleteSirenaOperation);
          return new DeleteSirenaCommand(factory, planScheduler);
        }
      case "request": return new RequestRightsCommand(requests);
      case "requests": return new GetRequestsListCommand(requests.db, requests, bot);
      case "responsible": return new GetResponsiblesListCommand(requests.db, requests, bot);
      case "revoke": return new RevokeRightsCommand(requests, bot);
      case FindSirenaCommand.NAME: {
        var factory = new FindSirenaPlanFactory(findSirenaOperation,bot);
        return new FindSirenaCommand(factory, planScheduler);}
      case "start": return new StartCommand(requests);
      case "subscribe": return new SubscribeCommand(requests.db);
      case "subscriptions": return new GetSubscriptionsListCommand(requests.db);
      case "unmute": return new UnmuteUserSignalCommand(requests, bot);
      case "unsubscribe": return new UnsubscribeCommand(requests.db);
      default:
        {
          throw new ArgumentException("Wrong command name: " + commandName, "commandName");
        }
    }
  }
}