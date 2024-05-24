using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;
public class CommandsCollectionInitializer
{
  private static List<string> commandNames = [MenuBotCommand.NAME,
  CreateSirenaCommand.NAME,
  CallSirenaCommand.NAME,
  DisplayUsersSirenasCommand.NAME,
  DisplaySirenaInfoCommand.NAME,
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
  private readonly IFactory<string, AbstractBotCommmand> factory;

  public CommandsCollectionInitializer(IFactory<string, AbstractBotCommmand> factory)
  {
    this.factory = factory;
  }

  public void Initialize(BotCommands botCommands)
  {
    botCommands.Clear();
    foreach (var commandName in commandNames)
    {
      var command = factory.Create(commandName);
      botCommands.Add(command);
    }
  }
}