using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class NoSirenaMessageBuilder : MessageBuilder
{
  private const string noSirenaError = "There is no sirena with title that contains search phrase: \"{0}\". Please try again with another search phrase";
  string key;
  public NoSirenaMessageBuilder(long chatId, string key) : base(chatId)
  {
    this.key = key;
  }

  public override SendMessage Build()
  {
    const string menuTitle = "🧾 Back to menu";

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddCallbackData(menuTitle, '/' + MenuBotCommand.NAME).EndRow();

    IReplyMarkup markup = new InlineKeyboardMarkup()
    {
      InlineKeyboard = keyboardBuilder.Build()
    };
    var message = string.Format(noSirenaError, key);
    return CreateDefault(message, markup);
  }
}
