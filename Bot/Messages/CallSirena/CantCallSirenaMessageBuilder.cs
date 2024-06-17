using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NotAllowedToCallSirenaMessageBuilder : LocalizedMessageBuilder
{
  private SirenRepresentation sirena;

  public NotAllowedToCallSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, SirenRepresentation sirena)
  :base(chatId,info,localizationProvider)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    const string notification = "You are not allowed to call sirena *\"{0}\"* with ID: `{1}`";
    string message = string.Format(notification,sirena.Title, sirena.Id);

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    .AddDisplayUserSirenasButton(Info)
    .AddDisplaySubscriptionsButton(Info).EndRow()
    .BeginRow().AddMenuButton(Info).EndRow().ToReplyMarkup();

    return CreateDefault(message, markup);
  }
}