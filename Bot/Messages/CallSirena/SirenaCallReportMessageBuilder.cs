using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class SirenaCallReportMessageBuilder : MessageBuilder
{
  private readonly int notifiedSubscribers;
  private readonly SirenRepresentation sirenRepresentation;

  public SirenaCallReportMessageBuilder(long chatId,int notifiedSubscribers, SirenRepresentation sirenRepresentation)
   : base(chatId)
  {
    this.notifiedSubscribers = notifiedSubscribers;
    this.sirenRepresentation = sirenRepresentation;
  }

  public override SendMessage Build()
  {
    const string notification = "{0} subscribers were notified";
    string message = string.Format(notification, notifiedSubscribers);
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
     .AddMenuButton().AddDeleteButton(sirenRepresentation.Id).EndRow()
     .ToReplyMarkup();
     
    return CreateDefault(message,markup);
  }
}
