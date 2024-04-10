using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Text;

namespace Hedgey.Sirena.Bot
{
  class HelpCommand : AbstractBotCommmand, IBotCommand
  {
    const string NAME = "help";
    const string DESCRIPTION = "";
    List<AbstractBotCommmand> commands;
    TelegramBot bot;
    public HelpCommand(TelegramBot bot, IEnumerable<AbstractBotCommmand> commands)
    : base(NAME, DESCRIPTION)
    {

      this.commands = commands != null ? new(commands) : new();
      this.bot = bot;
    }

    public async override void Execute(ICommandContext context)
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
        Text = builder.ToString()
      };
      Program.messageSender.Send(response);
    }
  }
}