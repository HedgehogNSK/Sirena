using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Sirena;

public record class CallbackRequestContext : IRequestContext
{
  public CallbackQuery Query { get; }
  private string argString;
  private string commandName;

  public CallbackRequestContext(CallbackQuery query)
  {
    this.Query = query;
    Extensions.TextTools.ExtractCommandAndArgs(query.Data, out commandName, out argString);
  }
  public string GetArgsString() => argString;
  public Chat GetChat() => Query.Message.Chat;
  public string GetCommandName() => commandName;
  public CultureInfo GetCultureInfo() => new(GetUser().LanguageCode);
  public Message GetMessage() => Query.Message;
  public string GetQuery() => Query.Data;
  public long GetTargetChatId() => Query.Message.Chat.Id;
  public User GetUser() => Query.From;
}