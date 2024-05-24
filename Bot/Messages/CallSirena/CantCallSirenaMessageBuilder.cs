using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class NotAllowedToCallSirenaMessageBuilder : MessageBuilder
{
  private SirenRepresentation sirena;

  public NotAllowedToCallSirenaMessageBuilder(long chatId, SirenRepresentation sirena)
  :base(chatId)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    const string notification = "You are not allowed to call sirena *\"{0}\"* with ID: `{1}`";
    string message = string.Format(notification,sirena.Title, sirena.Id);

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    .AddDisplayUserSirenasButton()
    .AddDisplaySubscriptionsButton().EndRow()
    .BeginRow().AddMenuButton().EndRow().ToReplyMarkup();

    return CreateDefault(message, markup);
  }
}