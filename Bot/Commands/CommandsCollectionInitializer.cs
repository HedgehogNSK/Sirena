namespace Hedgey.Sirena.Bot;
public class CommandsCollectionInitializer{
  private static List<string> commandNames = ["menu","create","call","list","search"
  ,"remove","subscribe","subscriptions","unsubscribe","mute","unmute","responsible"
  ,"delegate","revoke","request","requests","help","start"];
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