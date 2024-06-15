using System.Globalization;
using Hedgey.Sirena.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena;

public interface IRequestContext : IValidator<AbstractBotCommmand>
{
  string GetCommandName();
  string GetArgsString();
  User GetUser();
  Chat GetChat();
  Message GetMessage();
  long GetTargetChatId();
  CultureInfo GetCultureInfo();
}
