using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Text;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class HelpCommand : AbstractBotCommmand, IBotCommand
{
  public const string NAME = "help";
  public const string DESCRIPTION = null;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;
  private readonly IEnumerable<AbstractBotCommmand> commands;
  public HelpCommand(IEnumerable<AbstractBotCommmand> commands
  , ILocalizationProvider localizationProvider
  , IMessageSender messageSender)
  : base(NAME, DESCRIPTION)
  {
    IsPublic = false;
    this.commands = commands.Where(_command => _command.IsPublic);
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }

  public override void Execute(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    string commandsList = localizationProvider.Get("command.help.header", info);
    StringBuilder builder = new StringBuilder(commandsList).AppendLine();
    foreach (var commandName in commands.Select(_command => _command.Command))
    {
      builder.Append('/').Append(commandName.Replace("_","\\_")).Append(" - ");
      string description = localizationProvider.Get($"command.{commandName}.description", info);
      builder.Append(description).AppendLine();
    }
    long uid = context.GetUser().Id;
    var response = new SendMessage()
    {
      ChatId = uid,
      Text = builder.ToString(),
      ParseMode = ParseMode.Markdown
    };
    messageSender.Send(response);
  }
}