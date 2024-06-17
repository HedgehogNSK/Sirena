using Hedgey.Localization;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Utils.Keyboard;
using RxTelegram.Bot.Utils.Keyboard.Interfaces;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public static class MarkupShortcuts
{
  const string prefix = "menu.buttons.";
  public const string menuTitle = prefix + "menu.title";
  public const string callTitle = prefix + "call.title";
  public const string createTitle = prefix + "create.title";
  public const string getInfoTitle = prefix + "getInfo.title";
  public const string findTitle = prefix + "find.title";
  public const string subscribeTitle = prefix + "subscribe.title";
  public const string unsubscribeTitle = prefix + "unsubscribe.title";
  public const string deleteTitle = prefix + "delete.title";
  public const string displaySirenasTitle = prefix + "displaySirenas.title";
  public const string getReuqestsTitle = prefix + "getReuqests.title";
  public const string getResponsiblesTitle = prefix + "getResponsibles.title";
  public const string requestRightTitle = prefix + "requestRight.title";
  public const string subscriptionsTitle = prefix + "subscriptions.title";

  public static ILocalizationProvider? LocalizationProvider { get; set;}

  public static IInlineKeyboardRow AddButton(this IInlineKeyboardRow inlineKeyboardRow
    , string title, string commandName, string param = "")
  {
    string command = '/' + commandName;
    if (!string.IsNullOrEmpty(param))
      command += ' ' + param;
    return inlineKeyboardRow.AddCallbackData(title, command);
  }

  private static IInlineKeyboardRow AddLocalizedButton(this IInlineKeyboardRow inlineKeyboardRow
  , string title, CultureInfo info, string commandName, string param = ""){
    if(LocalizationProvider==null)
      throw new ArgumentNullException(nameof(LocalizationProvider));
    string localTitle = LocalizationProvider.Get(title, info);
    return inlineKeyboardRow.AddButton(localTitle, commandName, param);
  }

  public static IInlineKeyboardRow AddMenuButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(menuTitle,info, MenuBotCommand.NAME);
  public static IInlineKeyboardRow AddCallSirenaButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, ObjectId? sirenaId = null)
    => inlineKeyboardRow.AddLocalizedButton(callTitle, info, CallSirenaCommand.NAME, sirenaId?.ToString() ?? string.Empty);
  public static IInlineKeyboardRow AddCreateButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(createTitle, info, CreateSirenaCommand.NAME);
  public static IInlineKeyboardRow AddDeleteButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, ObjectId sirenaId, string title = deleteTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, DeleteSirenaCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddDisplaySubscriptionsButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, int count = 0, string title = subscriptionsTitle)
  {
    if (count != 0)
      title += $" [{count}]";
    return inlineKeyboardRow.AddLocalizedButton(title, info, GetSubscriptionsListCommand.NAME);
  }

  public static IInlineKeyboardRow AddDisplayUserSirenasButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info
    , int count = 0, string title = displaySirenasTitle)
  {
    if (count != 0)
      title += $" [{count}]";
    return inlineKeyboardRow.AddLocalizedButton(title, info, DisplayUsersSirenasCommand.NAME);
  }

  public static IInlineKeyboardRow AddFindButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(findTitle, info, FindSirenaCommand.NAME);
  public static IInlineKeyboardRow AddRequestButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info
  , ObjectId sirenaId, string title = requestRightTitle) 
    => inlineKeyboardRow.AddLocalizedButton(title, info, RequestRightsCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddSirenaInfoButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info
  , ObjectId sirenaId, string title = getInfoTitle) 
    => inlineKeyboardRow.AddLocalizedButton(title, info, DisplaySirenaInfoCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info)
   => inlineKeyboardRow.AddLocalizedButton(subscribeTitle, info, SubscribeCommand.NAME);
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, ObjectId sirenaId)
   => inlineKeyboardRow.AddLocalizedButton(subscribeTitle, info, SubscribeCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddUnsubscribeButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, ObjectId sirenaId)
    => inlineKeyboardRow.AddLocalizedButton(unsubscribeTitle, info, UnsubscribeCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddDisplayRequestsButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, ObjectId sirenaId, int count)
  {
    string title = string.Format(getReuqestsTitle, count);
    return inlineKeyboardRow.AddLocalizedButton(title, info, GetRequestsListCommand.NAME, sirenaId.ToString());
  }

  public static IInlineKeyboardRow AddDisplayResponsiblesButton(this IInlineKeyboardRow inlineKeyboardRow,CultureInfo info, ObjectId sirenaId, int count)
  {
    string title = string.Format(getResponsiblesTitle, count);
    return inlineKeyboardRow.AddLocalizedButton(title, info, GetResponsiblesListCommand.NAME, sirenaId.ToString());
  }

  public static IReplyMarkup CreateMenuButtonOnlyMarkup(CultureInfo info)
  {
    return new InlineKeyboardMarkup()
    {
      InlineKeyboard = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddMenuButton(info).EndRow().Build()
    };
  }
  public static IReplyMarkup ToReplyMarkup(this IInlineKeyboardBuilder builder)
  {
    return new InlineKeyboardMarkup()
    {
      InlineKeyboard = builder.Build()
    };
  }
}