using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class WrongSearchKeyFindSirenaMessageBuilder : MessageBuilder
{
  const string errorDescription = "*Please input search phrase*.\nSearch phrase has to be at least {0} symbols length but less than {1} symbols.";
  public WrongSearchKeyFindSirenaMessageBuilder(long chatId)
: base(chatId) { }

  public override SendMessage Build()
  {
    return CreateDefault(string.Format(errorDescription
      , ValidateSearchParamFindSirenaStep.MIN_SIMBOLS
      , ValidateSearchParamFindSirenaStep.MAX_SIMBOLS)
      , MarkupShortcuts.CreateMenuButtonOnlyMarkup());
  }
}

