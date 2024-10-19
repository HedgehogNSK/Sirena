using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
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
  public const string getInfoTitle = prefix + "get_info.title";
  public const string findTitle = prefix + "find.title";
  public const string subscribeTitle = prefix + "subscribe.title";
  public const string unsubscribeTitle = prefix + "unsubscribe.title";
  public const string deleteTitle = prefix + "delete.title";
  public const string displaySirenasTitle = prefix + "display_sirenas.title";
  public const string getReuqestsTitle = prefix + "get_requests.title";
  public const string getResponsiblesTitle = prefix + "get_responsibles.title";
  public const string requestRightTitle = prefix + "request_rights.title";
  public const string subscriptionsTitle = prefix + "subscriptions.title";

  public static ILocalizationProvider? LocalizationProvider { get; set; }

  public static IInlineKeyboardRow AddButton(this IInlineKeyboardRow inlineKeyboardRow
    , object title, string commandName, string param = "")
  {
    string command = '/' + commandName;
    if (!string.IsNullOrEmpty(param))
      command += ' ' + param;
    return inlineKeyboardRow.AddCallbackData(title.ToString(), command);
  }

  public static IInlineKeyboardRow AddLocalizedButton(this IInlineKeyboardRow inlineKeyboardRow
  , string textKey, CultureInfo info, string commandName, string param = "")
  {
    if (LocalizationProvider == null)
      throw new ArgumentNullException(nameof(LocalizationProvider), $"You have to set {nameof(MarkupShortcuts.LocalizationProvider)} field manually before use localized buttons");
    string localTitle = LocalizationProvider.Get(textKey, info);
    return inlineKeyboardRow.AddButton(localTitle, commandName, param);
  }

  public static IInlineKeyboardRow AddMenuButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(menuTitle, info, MenuBotCommand.NAME);
  public static IInlineKeyboardRow AddCallSirenaButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, ulong? sirenaId = null)
    => inlineKeyboardRow.AddLocalizedButton(callTitle, info, CallSirenaCommand.NAME, sirenaId != null ? Extensions.Converter.UlongToBase64URLHM(sirenaId.Value) : string.Empty);
  public static IInlineKeyboardRow AddCreateButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(createTitle, info, CreateSirenaCommand.NAME);
  public static IInlineKeyboardRow AddDeleteButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, ulong sirenaId, string title = deleteTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, DeleteSirenaCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
  public static IInlineKeyboardRow AddDisplaySubscriptionsButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, int count = 0, string title = subscriptionsTitle)
  {
    if (LocalizationProvider == null)
      throw new ArgumentNullException(nameof(LocalizationProvider));
    string localTitle = LocalizationProvider.Get(title, info);
    if (count != 0)
      title += $" [{count}]";
    return inlineKeyboardRow.AddButton(localTitle, GetSubscriptionsListCommand.NAME);
  }

  public static IInlineKeyboardRow AddDisplayUserSirenasButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
    , int count = 0, string title = displaySirenasTitle)
  {
    if (LocalizationProvider == null)
      throw new ArgumentNullException(nameof(LocalizationProvider));
    string localTitle = LocalizationProvider.Get(title, info);
    if (count != 0)
      localTitle += $" [{count}]";
    return inlineKeyboardRow.AddButton(localTitle, DisplayUsersSirenasCommand.NAME);
  }

  public static IInlineKeyboardRow AddFindButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(findTitle, info, FindSirenaCommand.NAME);
  public static IInlineKeyboardRow AddRequestButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
  , ulong sirenaId, string title = requestRightTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, RequestRightsCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
  public static IInlineKeyboardRow AddSirenaInfoButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
  , ulong sirenaId, string title = getInfoTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, DisplaySirenaInfoCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
   => inlineKeyboardRow.AddLocalizedButton(subscribeTitle, info, SubscribeCommand.NAME);
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, ulong sirenaId)
   => inlineKeyboardRow.AddLocalizedButton(subscribeTitle, info, SubscribeCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
  public static IInlineKeyboardRow AddUnsubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, ulong sirenaId)
    => inlineKeyboardRow.AddLocalizedButton(unsubscribeTitle, info, UnsubscribeCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
  public static IInlineKeyboardRow AddDisplayRequestsButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, ulong sirenaId, int count)
  {
    string title = string.Format(getReuqestsTitle, count);
    return inlineKeyboardRow.AddLocalizedButton(title, info, GetRequestsListCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
  }

  public static IInlineKeyboardRow AddDisplayResponsiblesButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, ulong sirenaId, int count)
  {
    string title = string.Format(getResponsiblesTitle, count);
    return inlineKeyboardRow.AddLocalizedButton(title, info, GetResponsiblesListCommand.NAME, Extensions.Converter.UlongToBase64URLHM(sirenaId));
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

  static public SendMessage CreateDefaultMessage(long chatId, string message, IReplyMarkup? replyMarkup = null)
    => new SendMessage()
    {
      ChatId = chatId,
      DisableNotification = true,
      ProtectContent = false,
      Text = message,
      ReplyMarkup = replyMarkup,
      ParseMode = ParseMode.Markdown
    };
}