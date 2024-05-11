using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class SirenaNotFoundMessageBuilder : MessageBuilder
{
  private readonly ObjectId id;
  public SirenaNotFoundMessageBuilder(long chatId, ObjectId id)
  : base(chatId)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    var markup = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow().AddFindButton().AddSubscribeButton().EndRow()
        .BeginRow().AddMenuButton().EndRow()
        .ToMarkup();

    const string noSirenaError = "*There is no Sirena with id:* `{0}`\nAttempt to subscribe is failed.";
    var message = string.Format(noSirenaError, id);
    return CreateDefault(message, markup);
  }
}