using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Text;

namespace Hedgey.Sirena.Bot
{
  class HelpCommand : BotCustomCommmand, IBotCommand
  {
    List<BotCustomCommmand> commands;
    TelegramBot bot;
    public HelpCommand(string name, string description, TelegramBot bot, IEnumerable<BotCustomCommmand> commands) : base(name, description)
    {

      this.commands = commands != null ? new(commands) : new();
      this.bot = bot;
    }

    public override void Execute(Message message)
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
      var response = new SendMessage()
      {
        ChatId = message.Chat.Id,
        Text = builder.ToString()
      };
      bot.SendMessage(response);
    }
  }
}