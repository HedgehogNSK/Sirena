using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Telegram.Bot;

public record CallbackRequestContext : IRequestContext
{
  public CallbackQuery Query { get; }
  private readonly string argString;
  private readonly string commandName;
  private readonly CultureInfo cultureInfo;

  public CallbackRequestContext(CallbackQuery query)
  {
    Query = query;
    Extensions.TextTools.ExtractCommandAndArgs(query.Data, out commandName, out argString);
    cultureInfo = new(Query.From.LanguageCode);
  }
  public string GetArgsString() => argString;
  public Chat GetChat() => Query.Message.Chat;
  public string GetCommandName() => commandName;
  public CultureInfo GetCultureInfo() => cultureInfo;
  public Message GetMessage() => Query.Message;
  public string GetQuery() => Query.Data;
  public long GetTargetChatId() => GetChat().Id;
  public User GetUser() => Query.From;
}