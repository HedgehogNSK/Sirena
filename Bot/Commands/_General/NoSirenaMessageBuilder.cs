using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NoSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, string key)
   : LocalizedMessageBuilder(chatId, info, localizationProvider)
{

  public override SendMessage Build()
  {
    string noSirenaByIdError = Localize("command.find.no_id");
    string message = string.Format(noSirenaByIdError, key);
    return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}