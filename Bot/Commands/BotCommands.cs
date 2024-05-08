namespace Hedgey.Sirena.Bot;

public class BotCommands
{
  List<AbstractBotCommmand> commands = [];
  public IEnumerable<AbstractBotCommmand> Commands => commands.AsEnumerable();

  public void Add(AbstractBotCommmand command) => commands.Add(command);
  public void AddRange(IEnumerable<AbstractBotCommmand> newCommands) => this.commands.AddRange(newCommands);
  public void Clear() => commands.Clear();
  public AbstractBotCommmand? GetCommmandOrNull(string commandName)
      => commands.FirstOrDefault(_command => string.CompareOrdinal(_command.Command, commandName) == 0);
  public AbstractBotCommmand? Find(Func<AbstractBotCommmand, bool> predicate)
   => commands.FirstOrDefault(predicate);
}
