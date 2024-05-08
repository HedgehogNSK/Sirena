using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot
{
  public abstract class AbstractBotCommmand : BotCommand, IBotCommand
  {
    public AbstractBotCommmand(string name, string description){
      Command = name;
      Description = description;
    }
    public abstract void Execute(IRequestContext context);
  }
}