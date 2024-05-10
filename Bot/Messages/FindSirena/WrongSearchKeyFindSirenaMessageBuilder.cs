using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class WrongSearchKeyFindSirenaMessageBuilder : MessageBuilder
{
  const string errorDescription = "*Please input search phrase*.\nSearch phrase has to be at least {0} symbols length but less than {1} symbols.";
  public WrongSearchKeyFindSirenaMessageBuilder(long chatId)
: base(chatId) { }

  public override SendMessage Build()
  {
    const string menuTitle = "🧾 Back to menu";
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddCallbackData(menuTitle, '/' + MenuBotCommand.NAME).EndRow();

    IReplyMarkup markup = new InlineKeyboardMarkup()
    {
      InlineKeyboard = keyboardBuilder.Build()
    };
    return CreateDefault(string.Format(errorDescription
      , ValidateSearchParamFindSirenaStep.MIN_SIMBOLS
      , ValidateSearchParamFindSirenaStep.MAX_SIMBOLS), markup);
  }
}

