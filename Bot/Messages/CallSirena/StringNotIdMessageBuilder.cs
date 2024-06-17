using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class StringNotIdMessageBuilder : LocalizedMessageBuilder
{
  private string param;

  public StringNotIdMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, string param)
  : base(chatId,info,localizationProvider)
  {
    this.chatId = chatId;
    this.param = param;
  }

  public override SendMessage Build()
  {
    const string paramIncorrect = "*Value is incorrect!*\n";
    const string notification = "Please provide Sirena ID to call. It has to be ID of a Sirena that you own or you are responsible for";
    string message = string.IsNullOrEmpty(param) ? notification : paramIncorrect + notification;

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddDisplayUserSirenasButton(Info).AddMenuButton(Info).EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }
}