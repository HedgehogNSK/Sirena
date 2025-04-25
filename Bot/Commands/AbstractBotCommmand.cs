using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot
{
  public abstract class AbstractBotCommmand : BotCommand, IBotCommand
  {
    public bool IsPublic { get; protected set; }
    protected AbstractBotCommmand(string name, string description)
    {
      Command = name;
      Description = description;
      IsPublic =  true;
    }
    public abstract void Execute(IRequestContext context);
  }
}