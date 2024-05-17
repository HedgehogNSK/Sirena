using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class ConfirmRemoveSirenaMessageBuilder : MessageBuilder
{
  private readonly SirenRepresentation sirena;

  public ConfirmRemoveSirenaMessageBuilder(long chatId, SirenRepresentation sirena) : base(chatId)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    const string question = "You are going to delete Sirena: *{0}*\n ID: `{1}`\n*Are your sure?* This action can't be canceled.";
    const string confirm = "✅ Yes";
    const string cancel = "⛔️ No";
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddSirenaInfoButton(sirena.Id,cancel)
      .AddCallbackData(confirm, true.ToString())
      .EndRow().ToReplyMarkup();

      var message = string.Format(question, sirena.Title, sirena.Id);
      return CreateDefault(message, markup);
  }
}