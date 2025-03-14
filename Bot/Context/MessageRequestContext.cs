using Hedgey.Extensions;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public record class MessageRequestContext : IRequestContext
{
  private Message message;
  private bool commandIsSet;
  private string commandName;
  private string argString;

  public MessageRequestContext(Message message)
  {
    this.message = message;
    commandIsSet = TextTools.ExtractCommandAndArgs(message.Text, out commandName, out argString);
  }

  public string GetArgsString() => argString;
  public Chat GetChat() => message.Chat;
  public string GetCommandName() => commandName;
  public CultureInfo GetCultureInfo() => new(GetUser().LanguageCode);
  public Message GetMessage() => message;
  public string GetQuery() => message.Text;
  public long GetTargetChatId() => message.From.Id;
  public User GetUser() => message.From;
  public bool IsCommandSet() => commandIsSet;
}