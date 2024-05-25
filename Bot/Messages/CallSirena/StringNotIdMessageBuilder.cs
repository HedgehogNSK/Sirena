using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class StringNotIdMessageBuilder : MessageBuilder
{
  private string param;

  public StringNotIdMessageBuilder(long chatId, string param)
  : base(chatId)
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
      .AddDisplayUserSirenasButton().AddMenuButton().EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }
}