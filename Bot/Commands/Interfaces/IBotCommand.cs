using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public interface IBotCommand{
  void Execute(IRequestContext context);
}