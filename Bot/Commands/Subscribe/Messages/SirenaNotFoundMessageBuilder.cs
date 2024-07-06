using Hedgey.Localization;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SirenaNotFoundMessageBuilder : LocalizedMessageBuilder
{
  private readonly ObjectId id;
  public SirenaNotFoundMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, ObjectId id)
  : base(chatId,info,localizationProvider)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
        .AddFindButton(Info).AddSubscribeButton(Info).AddMenuButton(Info)
        .EndRow().ToReplyMarkup();

    string noSirenaError = Localize("command.subscribe.notExists");
    var message = string.Format(noSirenaError, id);
    return CreateDefault(message, markup);
  }
}