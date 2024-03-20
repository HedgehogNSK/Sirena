using Hedgey.Sirena.Bot;
using System.Text;

namespace Hedgey.Sirena;

public static class BotCommands
{
  // create - create your own siren
  // delegate - delegate rights of turn on the siren to some user
  // list - shows a list of sirens that are being tracked
  // remove - remove your signal by number, or by id
  // mute - mute all notifications from siren
  // subscribe - Subscribes to track a certain siren
  // unsubscribe - Unsubscribes from certain siren
  // unsubscribeall - Unsubcribes from all tracked sirens.;

  static List<BotCustomCommmand> commands = [];
  static public IEnumerable<BotCustomCommmand> Commands => commands.AsEnumerable();

  static public void Add(BotCustomCommmand command) => commands.Add(command);
  static public void AddRange(IEnumerable<BotCustomCommmand> commands) => BotCommands.commands.AddRange(commands);

  public static bool Contains(string text, out BotCustomCommmand? command)
  {
    if (text[0] != '/')
    {
      command = null;
      return false;
    }
    string commandName = GetCommandName(text);
    command = commands.FirstOrDefault(_command => string.CompareOrdinal(_command.Command, commandName) == 0);

    return command != default;
  }

  private static string GetCommandName(string text)
  {
    StringBuilder builder = new StringBuilder();
    foreach (var character in text.Skip(1).TakeWhile(x => x != ' '))
    {
      builder.Append(character);
    }
    string commandName = builder.ToString();
    return commandName;
  }
}