using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena;

public interface ICommandContext
{
  string GetCommandName();
  string GetArgsString();
  User GetUser();
  Chat GetChat();
}
