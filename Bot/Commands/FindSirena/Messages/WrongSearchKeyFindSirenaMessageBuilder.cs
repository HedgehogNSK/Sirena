using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class WrongSearchKeyFindSirenaMessageBuilder : LocalizedMessageBuilder
{
  public WrongSearchKeyFindSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider)
: base(chatId,info,localizationProvider) { }

  public override SendMessage Build()
  {
    string errorDescription = Localize("command.find.input_phrase");
    return CreateDefault(string.Format(errorDescription
      , ValidateSearchParamFindSirenaStep.MIN_SIMBOLS
      , ValidateSearchParamFindSirenaStep.MAX_SIMBOLS)
      ,  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}