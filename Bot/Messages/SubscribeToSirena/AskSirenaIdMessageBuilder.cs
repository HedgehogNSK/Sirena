using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class AskSirenaIdMessageBuilder : LocalizedMessageBuilder
{
  public AskSirenaIdMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider)
: base(chatId,info,localizationProvider) { }

  public override SendMessage Build()
  {
    const string errorDescription = "*Please input Sirena ID*.\nYou have to input exact ID of Sirena that you are going to subscribe to.\n\nYou can use *Find* command to find a Sirena by title.";
    
    var markup = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow().AddFindButton(Info).AddMenuButton(Info).EndRow()
        .ToReplyMarkup();

    return CreateDefault(errorDescription, markup);
  }
}