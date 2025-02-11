using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Sirena;

public interface IRequestContext
{
  string GetArgsString();
  Chat GetChat();
  string GetCommandName();
  CultureInfo GetCultureInfo();
  Message GetMessage();
  string GetQuery();
  long GetTargetChatId();
  User GetUser();
}