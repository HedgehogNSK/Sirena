using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Telegram.Bot;

public record FallbackRequestContext : IRequestContext
{
  private readonly string command;
  private readonly string args;

  public IRequestContext InnerContext { get; }

  public FallbackRequestContext(IRequestContext context, string command, string? args = null)
  {
    InnerContext = context;
    this.command = command;
    this.args = args ?? string.Empty;
  }
  public string GetArgsString() => args;
  public Chat GetChat() => InnerContext.GetChat();
  public string GetCommandName() => command;
  public CultureInfo GetCultureInfo() => InnerContext.GetCultureInfo();
  public Message GetMessage() => InnerContext.GetMessage();
  public string GetQuery() => InnerContext.GetQuery();
  public long GetTargetChatId() => InnerContext.GetTargetChatId();
  public User GetUser() => InnerContext.GetUser();
}
