using Hedgey.Sirena.Bot.Operations;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class MenuMessageBuilder
{
  const string message = "Please select one of the options";
  private long chatId;
  private bool userHasSirenas = false;
  private bool userSubscribed = false;
  private UserStatistics? result = null;

  public MenuMessageBuilder(long chatId)
  {
    this.chatId = chatId;
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
  public SendMessage Build()
  {
    const string searchTitle = "🔎 Find";
    const string searchCallback = "/search";
    const string createTitle = "🆕 Create";
    const string createCallback = "/create";
    const string listTitle = "🖥 Your Sirenas";
    const string listCallback = "/list";
    const string subscribeTitle = "🔔 Subscribe";
    const string subscribeCallback = "/subscribe";
    const string subscriptionsTitle = "👀 Subscriptions";
    const string subscriptionsCallback = "/subscriptions";

    var searchButton = new InlineKeyboardButton()
    {
      Text = searchTitle,
      CallbackData = searchCallback
    };
    var userSirenasManageButtons = new List<InlineKeyboardButton>(){
        new InlineKeyboardButton()
    {
      Text = createTitle,
       CallbackData =createCallback
    }};
    if (userHasSirenas)
      userSirenasManageButtons.Add(new InlineKeyboardButton()
      {
        Text = listTitle + ((result != null && result.SirenasCount != 0) ? $" [{result.SirenasCount}]" : string.Empty),
        CallbackData = listCallback
      });
    var subscriptionManageButtons = new List<InlineKeyboardButton>(){
      searchButton,
    new InlineKeyboardButton()
    {
      Text = subscribeTitle,
      CallbackData = subscribeCallback
    }};
    if (userSubscribed)
      subscriptionManageButtons.Add(new InlineKeyboardButton()
      {
        Text = subscriptionsTitle + ((result != null && result.Subscriptions != 0) ? $" [{result.Subscriptions}]" : string.Empty),
        CallbackData = subscriptionsCallback
      });

    InlineKeyboardMarkup markup = new InlineKeyboardMarkup()
    {
      InlineKeyboard = [
        subscriptionManageButtons,
         userSirenasManageButtons,
         ]
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