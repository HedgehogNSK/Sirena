using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SirenaCallServiceMessageBuilder : MessageBuilder
{
  private readonly User initiator;
  private readonly SirenRepresentation sirena;

  public SirenaCallServiceMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, User initiator, SirenRepresentation sirena)
  : base(chatId, info, localizationProvider)
  {
    this.initiator = initiator;
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    string notificationBase = Localize("command.call.last_call.2");
    string userName = BotTools.GetDisplayName(initiator);
    long uid = initiator.Id;
    string notification = string.Format(notificationBase, sirena.Title, userName, uid);
    var markupBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    .AddUnsubscribeButton(Info, sirena)
    .AddMuteButton(Info, initiator, sirena)
    .EndRow()
    .ToReplyMarkup();
    var message = new SendMessage()
    {
      ChatId = ChatID,
      DisableNotification = true,
      ProtectContent = false,
      Text = notification,
      ReplyMarkup = markupBuilder,
      ParseMode = ParseMode.Markdown
    };
    return message;
  }
  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<long,IRequestContext, SirenRepresentation, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(long targetUID,IRequestContext context, SirenRepresentation sirena)
    {
      var info = context.GetCultureInfo();
      var user = context.GetUser();
      return new SirenaCallServiceMessageBuilder(targetUID, info, localizationProvider, user, sirena);
    }
  }
}