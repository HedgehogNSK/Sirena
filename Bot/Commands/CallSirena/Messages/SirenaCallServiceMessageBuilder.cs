using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SirenaCallServiceMessageBuilder : LocalizedMessageBuilder
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

    var message = new SendMessage()
    {
      ChatId = chatId,
      DisableNotification = true,
      ProtectContent = false,
      Text = notification,
      ReplyMarkup = null,
      ParseMode = ParseMode.Markdown
    };
    return message;
  }
  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      var user = context.GetUser();
      return new SirenaCallServiceMessageBuilder(chatId, info, localizationProvider, user, sirena);
    }
  }
}