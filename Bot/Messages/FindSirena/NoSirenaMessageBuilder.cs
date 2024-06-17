using Hedgey.Localization;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NoSirenaMessageBuilder : LocalizedMessageBuilder
{
  private readonly ObjectId id;
  string key =string.Empty;
  public NoSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, string key) : base(chatId,info,localizationProvider)
  {
    this.key = key;
  }
  public NoSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, ObjectId id) : base(chatId,info,localizationProvider)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    const string noSirenaByKeyError = "There is no Sirena with title that contains: \"{0}\".\nPlease try another key phrase";
    const string noSirenaByIdError = "There is no Sirena with this ID: \"{0}\".\nPlease try another ID phrase";
    string message = id != default ? string.Format(noSirenaByIdError, id)
      : string.Format(noSirenaByKeyError, key);
    return CreateDefault(message,  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}