using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public interface IBotCommand{
  void Execute(Message message);
}