using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena;

public record class MessageCommandContext : ICommandContext
{
  private User user;
  private Chat chat;
  private string argString;
  private string commandName;

  public MessageCommandContext(Message message)
  {
    user = message.From;
    chat = message.Chat;
    Extensions.TextTools.ExtractCommandAndArgs(message.Text, out commandName, out argString);
  }
  public string GetArgsString() => argString;
  public Chat GetChat() => chat;
  public string GetCommandName() => commandName;
  public User GetUser() => user;
}
