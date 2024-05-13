using Hedgey.Sirena.Bot.Operations;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class MenuMessageBuilder : MessageBuilder
{
  const string message = "Bot menu";
  private bool userHasSirenas = false;
  private bool userSubscribed = false;
  private UserStatistics? result = null;

  public MenuMessageBuilder(long chatId) : base(chatId)
  {
  }

  public MenuMessageBuilder UserHasSirenas(bool userHasSirenas)
  {
    this.userHasSirenas = userHasSirenas;
    return this;
  }
  public MenuMessageBuilder UserHasSubcriptions(bool userSubscribed)
  {
    this.userSubscribed = userSubscribed;
    return this;
  }
  internal MenuMessageBuilder AddUserStatistics(UserStatistics result)
  {
    this.result = result;
    UserHasSirenas(result.SirenasCount != 0);
    UserHasSubcriptions(result.Subscriptions != 0);
    return this;
  }
  public override SendMessage Build()
  {
    const char slash = '/';
    const string createTitle = "🆕 Create";
    const string listTitle = "🖥 Your Sirenas";
    const string subscriptionsTitle = "👀 Subscriptions";

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    .AddFindButton()
    .AddSubscribeButton();

    if (userSubscribed)
    {
      var title = subscriptionsTitle + ((result != null && result.Subscriptions != 0) ?
            $" [{result.Subscriptions}]" : string.Empty);
      keyboardBuilder.EndRow().BeginRow().AddCallbackData(title, slash + GetSubscriptionsListCommand.NAME);
    }

    keyboardBuilder.EndRow()
    .BeginRow()
    .AddCallbackData(createTitle, slash + CreateSirenaCommand.NAME);

    if (userHasSirenas)
    {
      var title = listTitle + ((result != null && result.SirenasCount != 0) ?
            $" [{result.SirenasCount}]" : string.Empty);
      keyboardBuilder.AddCallbackData(title, slash + ListUserSignalsCommand.NAME);
    }

    IReplyMarkup markup = new InlineKeyboardMarkup()
    {
      InlineKeyboard = keyboardBuilder.EndRow().Build()
    };

    return new SendMessage
    {
      ChatId = chatId,
      Text = message,
      ReplyMarkup = markup,
      ProtectContent = false,
      DisableNotification = true,
    };
  }
}