using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;

namespace Hedgey.Sirena;

public interface IRequestContext
{
  string GetCommandName();
  string GetArgsString();
  User GetUser();
  Chat GetChat();
  Message GetMessage();
  long GetTargetChatId();
  CultureInfo GetCultureInfo();
}