using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class ConfirmRemoveSirenaMessageBuilder : MessageBuilder
{
  private readonly SirenaData sirena;

  public ConfirmRemoveSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, SirenaData sirena) : base(chatId, info, localizationProvider)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    string question = Localize("command.delete.notification");
    string cancel = "command.delete.cancel";
    string confirm = Localize("command.delete.confirm");
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddSirenaInfoButton(Info, sirena.ShortHash, cancel)
      .AddCallbackData(confirm, true.ToString())
      .EndRow().ToReplyMarkup();

    var message = string.Format(question, sirena.Title, sirena.ShortHash);
    return CreateDefault(message, markup);
  }
  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, SirenaData, ConfirmRemoveSirenaMessageBuilder>
  {
    public ConfirmRemoveSirenaMessageBuilder Create(IRequestContext context, SirenaData sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new ConfirmRemoveSirenaMessageBuilder(chatId, info, localizationProvider, sirena);
    }
  }
}