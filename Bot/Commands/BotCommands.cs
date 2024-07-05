namespace Hedgey.Sirena.Bot;

public class BotCommands
{
  List<AbstractBotCommmand> commands = [];
  public IEnumerable<AbstractBotCommmand> Commands => commands.AsEnumerable();

  public void Add(AbstractBotCommmand command) => commands.Add(command);
  public void AddRange(IEnumerable<AbstractBotCommmand> newCommands) => this.commands.AddRange(newCommands);
  public void Clear() => commands.Clear();
  public bool TryGetCommand(string name, out AbstractBotCommmand? command)
  {
    command = Commands.FirstOrDefault(x => x.Command.Equals(name));
    return command != null;
  }
}
