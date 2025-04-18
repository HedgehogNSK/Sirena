using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SirenaCallServiceMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, User initiator
  , SirenaData sirena, SirenaActivation callInfo)
  : MessageBuilder(chatId, info, localizationProvider)
{
  private readonly User initiator = initiator;
  private readonly SirenaData sirena = sirena;
  private readonly SirenaActivation callInfo = callInfo;

  public override SendMessage Build()
  {
    string notificationBase = Localize("command.call.last_call.2");
    string userName = BotTools.GetDisplayName(initiator);
    long uid = initiator.Id;
    string notification = string.Format(notificationBase, sirena.Title, userName, uid);
    int[] reaction = [
       128591, //code for: 🙏
      128175, //code for: 💢
    ];
    var markupBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddReactButton(Info, callInfo.Id, reaction[0])
      .AddReactButton(Info, callInfo.Id, reaction[1])
      .EndRow()
      .BeginRow()
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
    : IFactory<IRequestContext, ServiceMessageData, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context, ServiceMessageData data)
    {
      var info = context.GetCultureInfo();
      var initiator = context.GetUser();
      return new SirenaCallServiceMessageBuilder(data.receiverId, info
        , localizationProvider, initiator, data.sirena, data.callInfo);
    }
  }
}