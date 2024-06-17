using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SirenaCallReportMessageBuilder : LocalizedMessageBuilder
{
  private readonly int notifiedSubscribers;
  private readonly SirenRepresentation sirenRepresentation;

  public SirenaCallReportMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider,int notifiedSubscribers, SirenRepresentation sirenRepresentation)
   : base(chatId,info,localizationProvider)
  {
    this.notifiedSubscribers = notifiedSubscribers;
    this.sirenRepresentation = sirenRepresentation;
  }

  public override SendMessage Build()
  {
    const string notification = "{0} subscribers were notified";
    string message = string.Format(notification, notifiedSubscribers);
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
     .AddMenuButton(Info).AddDeleteButton(Info,sirenRepresentation.Id).EndRow()
     .ToReplyMarkup();
     
    return CreateDefault(message,markup);
  }
}
