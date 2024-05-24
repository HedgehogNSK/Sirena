using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class SirenaCallServiceMessageBuilder : MessageBuilder
{
  private readonly User initiator;
  private readonly SirenRepresentation sirena;

  public SirenaCallServiceMessageBuilder(long chatId, User initiator, SirenRepresentation sirena)
  :base(chatId)
  {
    this.initiator = initiator;
    this.sirena = sirena;
  }

  public override SendMessage Build()
  { 
    const string notificationBase = "*\"{0}\"*\n _Called by_ {1}.";
    string userName = BotTools.GetUsername(initiator);
    long uid = initiator.Id;
    string notification = string.Format(notificationBase, sirena.Title,userName , uid);

    var message = new SendMessage(){
      ChatId = chatId,
      DisableNotification = true,
      ProtectContent = false,
      Text = notification,
      ReplyMarkup = null,
      ParseMode = RxTelegram.Bot.Interface.BaseTypes.Enums.ParseMode.Markdown
    };
    return message;
  }
}