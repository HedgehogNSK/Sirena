using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class NoSirenaMessageBuilder : MessageBuilder
{
  private readonly ObjectId id;
  string key;
  public NoSirenaMessageBuilder(long chatId, string key) : base(chatId)
  {
    this.key = key;
  }
  public NoSirenaMessageBuilder(long chatId, ObjectId id) : base(chatId)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    const string noSirenaByKeyError = "There is no Sirena with title that contains: \"{0}\".\nPlease try another key phrase";
    const string noSirenaByIdError = "There is no Sirena with this ID: \"{0}\".\nPlease try another ID phrase";
    string message = id != default ? string.Format(noSirenaByIdError, id)
      : string.Format(noSirenaByKeyError, key);
    return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup());
  }
}