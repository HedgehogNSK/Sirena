using Hedgey.Sirena.Bot.Operations;
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
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    .AddFindButton()
    .AddSubscribeButton();

    if (userSubscribed)
      keyboardBuilder.EndRow().BeginRow().AddDisplaySubscriptionsButton(result?.Subscriptions ?? 0);

    keyboardBuilder.EndRow()
    .BeginRow()
    .AddCreateButton();

    if (userHasSirenas)
      keyboardBuilder.AddDisplayUserSirenasButton(result?.SirenasCount ?? 0);

    IReplyMarkup markup = keyboardBuilder.EndRow().ToReplyMarkup();
    return CreateDefault(message, markup);
  }
}