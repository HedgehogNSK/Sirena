using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Text;

namespace Hedgey.Sirena.Bot
{
  class HelpCommand : AbstractBotCommmand, IBotCommand
  {
    public const string NAME = "help";
    public const string DESCRIPTION = "";
    List<AbstractBotCommmand> commands;
    public HelpCommand(IEnumerable<AbstractBotCommmand> commands)
    : base(NAME, DESCRIPTION)
    {
      this.commands = commands != null ? new(commands) : new();
    }

    public override void Execute(IRequestContext context)
    {
      StringBuilder builder = new StringBuilder("List of commands:\n");
      foreach (var command in commands)
      {
        builder.Append('/');
        builder.Append(command.Command);
        builder.Append(" - ");
        builder.Append(command.Description);
        builder.AppendLine();
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
  }
}