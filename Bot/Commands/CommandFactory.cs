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
  private readonly SirenaOperations sirenaOperation;
  private readonly IGetUserInformation getUserInformation;
  private readonly IMessageSender messageSender;

  public CommandFactory(FacadeMongoDBRequests requests, TelegramBot bot
  , BotCommands botCommands, PlanScheduler planScheduler, IMessageSender messageSender)
  {
    this.requests = requests;
    this.db = requests.db;
    this.bot = bot;
    this.botCommands = botCommands;
    this.planScheduler = planScheduler;
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    users = db.GetCollection<UserRepresentation>("users");
    sirenaOperation = new SirenaOperations(sirens, users);
    getUserInformation = new GetUserInformation(bot);
    this.messageSender = messageSender;
  }
  public AbstractBotCommmand Create(string commandName)
  {
    switch (commandName)
    {
      case MenuBotCommand.NAME:
        {
          IGetUserOverviewAsync getUserOverview = new GetUserStatsOperationAsync(sirens);
          return new MenuBotCommand(getUserOverview);
        }
      case CallSirenaCommand.NAME: return new CallSirenaCommand(requests);
      case CreateSirenaCommand.NAME:
        {
          var getUserStats = new GetUserOperationAsync(users, requests);
          var createSiren = new CreateSirenaOperationAsync(sirens, users);
          var factory = new CreateSirenaPlanFactory(getUserStats, createSiren);
          return new CreateSirenaCommand(factory, planScheduler);
        }
      case DelegateRightsCommand.NAME: return new DelegateRightsCommand(requests.db, requests, bot);
      case HelpCommand.NAME: return new HelpCommand(botCommands.Commands);
      case DisplayUsersSirenasCommand.NAME: return new DisplayUsersSirenasCommand(sirenaOperation,messageSender);
      case MuteUserSignalCommand.NAME: return new MuteUserSignalCommand(requests, bot);
      case DeleteSirenaCommand.NAME:
        {
          var factory = new DeleteSirenaPlanFactory(sirenaOperation, sirenaOperation, sirenaOperation);
          return new DeleteSirenaCommand(factory, planScheduler);
        }
      case RequestRightsCommand.NAME: return new RequestRightsCommand(requests);
      case GetRequestsListCommand.NAME: return new GetRequestsListCommand(requests.db, requests, bot);
      case GetResponsiblesListCommand.NAME: return new GetResponsiblesListCommand(requests.db, requests, bot);
      case RevokeRightsCommand.NAME: return new RevokeRightsCommand(requests, bot);
      case FindSirenaCommand.NAME: {
        var factory = new FindSirenaPlanFactory(sirenaOperation,bot);
        return new FindSirenaCommand(factory, planScheduler);}
      case StartCommand.NAME: return new StartCommand(requests);
      case SubscribeCommand.NAME:{
         var factory = new SubscribeSirenaPlanFactory(sirenaOperation);
         return new SubscribeCommand(factory,planScheduler);
         }
      case GetSubscriptionsListCommand.NAME: 
        return new GetSubscriptionsListCommand(messageSender,sirenaOperation,getUserInformation);
      case UnmuteUserSignalCommand.NAME: return new UnmuteUserSignalCommand(requests, bot);
      case UnsubscribeCommand.NAME: {
        var factory = new UnsubscribeSirenaPlanFactory(getUserInformation, sirenaOperation, sirenaOperation);
        return new UnsubscribeCommand(factory, planScheduler);}
      case DisplaySirenaInfoCommand.NAME: {
        var factory = new GetSirenaInfoPlanFactory(sirenaOperation);
        return new DisplaySirenaInfoCommand(factory, planScheduler);}

      default:
        {
          throw new ArgumentException("Wrong command name: " + commandName, "commandName");
        }
    }
  }
}