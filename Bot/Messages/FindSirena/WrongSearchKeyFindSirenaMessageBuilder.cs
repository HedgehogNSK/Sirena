using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class WrongSearchKeyFindSirenaMessageBuilder : LocalizedMessageBuilder
{
  const string errorDescription = "*Please input search phrase*.\nSearch phrase has to be at least {0} symbols length but less than {1} symbols.";
  public WrongSearchKeyFindSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider)
: base(chatId,info,localizationProvider) { }

  public override SendMessage Build()
  {
    return CreateDefault(string.Format(errorDescription
      , ValidateSearchParamFindSirenaStep.MIN_SIMBOLS
      , ValidateSearchParamFindSirenaStep.MAX_SIMBOLS)
      ,  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}

