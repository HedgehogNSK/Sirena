using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public class MenuMessageBuilder : MessageBuilder
{
  string menuLocalKey = "command.menu.title";
  private bool userHasSirenas = false;
  private bool userSubscribed = false;
  private UserStatistics? result = null;

  public MenuMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider)
  : base(chatId, info,localizationProvider)
  { }

  public MenuMessageBuilder SetText(string localizationKey)
  {
    menuLocalKey = localizationKey;
    return this;
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
    .AddFindButton(Info)
    .AddSubscribeButton(Info);

    if (userSubscribed)
      keyboardBuilder.EndRow().BeginRow().AddDisplaySubscriptionsButton(Info, result?.Subscriptions ?? 0);

    keyboardBuilder.EndRow()
    .BeginRow()
    .AddCreateButton(Info);

    if (userHasSirenas)
      keyboardBuilder.AddDisplayUserSirenasButton(Info,result?.SirenasCount ?? 0);

    IReplyMarkup markup = keyboardBuilder.EndRow().ToReplyMarkup();
    string localizedMessage = Localize(menuLocalKey);
    return CreateDefault(localizedMessage, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, UserStatistics, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context, UserStatistics userStats)
    {
      CultureInfo info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      return new MenuMessageBuilder(chatId, info, localizationProvider)
        .AddUserStatistics(userStats);
    }
  }
}