using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class StringNotIdMessageBuilder : LocalizedMessageBuilder
{
  private string param;

  public StringNotIdMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, string param)
  : base(chatId, info, localizationProvider)
  {
    this.chatId = chatId;
    this.param = param;
  }

  public override SendMessage Build()
  {
    string paramIncorrect = Localize("command.call.error.value_not_id");
    string notification = Localize("command.call.ask_sirena_id");
    string message = string.IsNullOrEmpty(param) ? notification : paramIncorrect + notification;

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddDisplayUserSirenasButton(Info).AddMenuButton(Info).EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }
  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, string, StringNotIdMessageBuilder>
  {
    public StringNotIdMessageBuilder Create(IRequestContext context, string param)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new StringNotIdMessageBuilder(chatId, info, localizationProvider, param);
    }
  }
}