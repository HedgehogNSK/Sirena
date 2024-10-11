using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NoSirenaMessageBuilder : LocalizedMessageBuilder
{
  private readonly ulong id;
  string key = string.Empty;
  public NoSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, string key) : base(chatId, info, localizationProvider)
  {
    this.key = key;
  }
  public NoSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, ulong id) : base(chatId, info, localizationProvider)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    string noSirenaByKeyError = Localize("command.find.no_title");
    string noSirenaByIdError = Localize("command.find.no_id");
    string message = id != default ? string.Format(noSirenaByIdError, id)
      : string.Format(noSirenaByKeyError, key);
    return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}