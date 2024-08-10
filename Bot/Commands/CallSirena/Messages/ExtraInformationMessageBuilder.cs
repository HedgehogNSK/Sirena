using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

internal class ExtraInformationMessageBuilder : LocalizedMessageBuilder
{
  private SirenRepresentation sirena;

  public ExtraInformationMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, SirenRepresentation sirena)
   : base(chatId,info,localizationProvider)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    string notification = Localize("command.call.extra_info");
    
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddCallSirenaButton(Info,sirena.Id).AddMenuButton(Info).EndRow()
      .ToReplyMarkup();

    return CreateDefault(notification,markup);
  }
}