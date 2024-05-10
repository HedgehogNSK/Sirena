namespace Hedgey.Sirena.Bot;
public class CommandsCollectionInitializer{
  private static List<string> commandNames = [MenuBotCommand.NAME,
  CreateSirenaCommand.NAME,
  CallSirenaCommand.NAME,
  ListUserSignalsCommand.NAME,
  FindSirenaCommand.NAME,
  DeleteSirenaCommand.NAME,
  SubscribeCommand.NAME,
  GetSubscriptionsListCommand.NAME,
  UnsubscribeCommand.NAME,
  MuteUserSignalCommand.NAME,
  UnmuteUserSignalCommand.NAME,
  GetResponsiblesListCommand.NAME,
  DelegateRightsCommand.NAME,
  RevokeRightsCommand.NAME,
  RequestRightsCommand.NAME,
  GetRequestsListCommand.NAME,
  HelpCommand.NAME,
  StartCommand.NAME,];
  private readonly BotCommands botCommands;
  private readonly CommandFactory factory;

  public CommandsCollectionInitializer(BotCommands botCommands, CommandFactory factory)
  {
    this.botCommands = botCommands;
    this.factory = factory;
  }

  public void Initialize(){
    botCommands.Clear();
     foreach(var commandName in commandNames)
     {
      var command = factory.Create(commandName);
      botCommands.Add(command);
     }
  }
}