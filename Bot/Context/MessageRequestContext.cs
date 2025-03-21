using Hedgey.Extensions;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Telegram.Bot;
public record MessageRequestContext : IRequestContext
{
  public Message Message { get; }
  private readonly bool commandIsSet;
  private readonly string commandName;
  private readonly string argString;
  private readonly CultureInfo cultureInfo;

  public MessageRequestContext(Message message)
  {
    Message = message;
    commandIsSet = TextTools.ExtractCommandAndArgs(message.Text, out commandName, out argString);
    cultureInfo = new(message.From.LanguageCode);
  }

  public string GetArgsString() => argString;
  public Chat GetChat() => Message.Chat;
  public string GetCommandName() => commandName;
  public CultureInfo GetCultureInfo() => cultureInfo; 
  public Message GetMessage() => Message;
  public string GetQuery() => Message.Text;
  public long GetTargetChatId() => GetChat().Id;
  public User GetUser() => Message.From;
  public bool IsCommandSet() => commandIsSet;
}