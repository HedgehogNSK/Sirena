using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SirenaCallReportMessageBuilder : MessageBuilder
{
  private readonly int notifiedSubscribers;
  private readonly SirenRepresentation sirenRepresentation;

  public SirenaCallReportMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, int notifiedSubscribers, SirenRepresentation sirenRepresentation)
   : base(chatId, info, localizationProvider)
  {
    this.notifiedSubscribers = notifiedSubscribers;
    this.sirenRepresentation = sirenRepresentation;
  }

  public override SendMessage Build()
  {
    string notification = Localize("command.call.success");
    string message = string.Format(notification, notifiedSubscribers);
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
     .AddMenuButton(Info).AddDeleteButton(Info, sirenRepresentation.ShortHash).EndRow()
     .ToReplyMarkup();

    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, int, SirenRepresentation, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context, int notifiedCount, SirenRepresentation sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new SirenaCallReportMessageBuilder(chatId, info, localizationProvider, notifiedCount, sirena);
    }
  }
}