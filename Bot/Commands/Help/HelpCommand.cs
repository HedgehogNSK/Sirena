using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Text;

namespace Hedgey.Sirena.Bot
{
  class HelpCommand : AbstractBotCommmand, IBotCommand
  {
    public const string NAME = "help";
    public const string DESCRIPTION = "";
    private readonly ILocalizationProvider localizationProvider;
    List<AbstractBotCommmand> commands;
    public HelpCommand(IEnumerable<AbstractBotCommmand> commands
    , ILocalizationProvider localizationProvider)
    : base(NAME, DESCRIPTION)
    {
      this.commands = commands != null ? new(commands) : new();
      this.localizationProvider = localizationProvider;
    }

    public override void Execute(IRequestContext context)
    {
      var info = context.GetCultureInfo();
      string commandsList = localizationProvider.Get("command.help.header",info);
      StringBuilder builder = new StringBuilder(commandsList).AppendLine();
      foreach (var command in commands)
      {
        builder.Append('/').Append(command.Command).Append(" - ");
        string description = localizationProvider.Get($"command.{command.Command}.description",info);
        builder.Append(description).AppendLine();
      }
      long uid = context.GetUser().Id;
      var response = new SendMessage()
      {
        ChatId = uid,
        Text = builder.ToString(),
        ParseMode = ParseMode.Markdown
      };
      Program.botProxyRequests.Send(response);
    }
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<HelpCommand>(container)
  { }
  }
  
}