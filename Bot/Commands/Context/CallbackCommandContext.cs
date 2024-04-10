using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena;

public class CallbackCommandContext : ICommandContext
{
  private User user;
  private Chat chat;
  private string argString;
  private string commandName;

  public CallbackCommandContext(CallbackQuery query)
  {
    user = query.Message.From;
    chat = query.Message.Chat;
    Extensions.TextTools.ExtractCommandAndArgs(query.Data, out commandName, out argString);
  }
  public string GetArgsString() => argString;

  public Chat GetChat() => chat;

  public string GetCommandName()
=> commandName;
  public User GetUser() => user;
}