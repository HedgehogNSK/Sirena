using Hedgey.Sirena.Bot;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena;

public record class CommandContext : IRequestContext
{
  private User user;
  private Chat chat;
  private string argString;
  private string commandName;

  public CommandContext(User user, Chat chat, string commandName, string argString )
  {
    this.user = user;
    this.chat = chat;
    this.commandName = commandName;
    this.argString = argString;
  }
  public string GetArgsString() => argString;
  public Chat GetChat() => chat;
  public string GetCommandName() => commandName;

  public long GetTargetChatId() => user.Id;

  public User GetUser() => user;
  public bool IsValid(AbstractBotCommmand command)
  {
    return command.Command == commandName;
  }
}