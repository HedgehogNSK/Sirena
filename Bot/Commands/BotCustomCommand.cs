using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot
{
  public abstract class BotCustomCommmand : BotCommand, IBotCommand
  {
    public BotCustomCommmand(string name, string description){
      Command = name;
      Description = description;
    }
    public abstract void Execute(Message message);
  }
}