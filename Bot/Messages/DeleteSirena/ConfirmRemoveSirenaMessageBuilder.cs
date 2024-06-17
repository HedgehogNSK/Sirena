using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class ConfirmRemoveSirenaMessageBuilder : LocalizedMessageBuilder
{
  private readonly SirenRepresentation sirena;

  public ConfirmRemoveSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, SirenRepresentation sirena) : base(chatId,info,localizationProvider)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    const string question = "You are going to delete Sirena: *{0}*\n ID: `{1}`\n*Are your sure?* This action can't be canceled.";
    const string confirm = "✅ Yes";
    const string cancel = "⛔️ No";
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddSirenaInfoButton(Info,sirena.Id,cancel)
      .AddCallbackData(confirm, true.ToString())
      .EndRow().ToReplyMarkup();

      var message = string.Format(question, sirena.Title, sirena.Id);
      return CreateDefault(message, markup);
  }
}