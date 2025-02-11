using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Sirena;

public class CallbackRequestContext : IRequestContext
{
  CallbackQuery query;
  private string argString;
  private string commandName;

  public CallbackRequestContext(CallbackQuery query)
  {
    this.query = query;
    Extensions.TextTools.ExtractCommandAndArgs(query.Data, out commandName, out argString);
  }
  public string GetArgsString() => argString;
  public Chat GetChat() => query.Message.Chat;
  public string GetCommandName() => commandName;
  public CultureInfo GetCultureInfo() => new(GetUser().LanguageCode);
  public Message GetMessage() => query.Message;
  public string GetQuery() => query.Data;
  public long GetTargetChatId() => query.From.Id;
  public User GetUser() => query.From;
}