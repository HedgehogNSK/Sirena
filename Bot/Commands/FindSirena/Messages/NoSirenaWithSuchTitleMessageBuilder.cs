using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NoSirenaWithSuchTitleMessageBuilder : LocalizedMessageBuilder
{
  string title = string.Empty;
  public NoSirenaWithSuchTitleMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, string title) 
  : base(chatId, info, localizationProvider)
  {
    this.title = title;
  }
  public override SendMessage Build()
  {
    string noSirenaByKeyError = Localize("command.find.no_title");
    string message = string.Format(noSirenaByKeyError, title);
    return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}