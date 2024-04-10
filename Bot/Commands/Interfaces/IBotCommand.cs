namespace Hedgey.Sirena.Bot;

public interface IBotCommand{
  void Execute(ICommandContext message);
}