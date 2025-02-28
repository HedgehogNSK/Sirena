using Hedgey.Localization;
using Hedgey.Sirena.Database;
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
  public const string muteTitle = prefix + "mute_user.title";
  public const string subscribeTitle = prefix + "subscribe.title";
  public const string unsubscribeTitle = prefix + "unsubscribe.title";
  public const string deleteTitle = prefix + "delete.title";
  public const string displaySirenasTitle = prefix + "display_sirenas.title";
  public const string getReuqestsTitle = prefix + "requests.title";
  public const string getResponsiblesTitle = prefix + "get_responsibles.title";
  public const string requestRightTitle = prefix + "request_rights.title";
  public const string retryTitle = prefix + "anotherTry.title";
  public const string subscriptionsTitle = prefix + "subscriptions.title";

  public static ILocalizationProvider? LocalizationProvider { get; set; }
  public const char Previous = '⬅';
  public const char Next = '➡';

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
  public static IInlineKeyboardRow AddCallSirenaButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string shortHash = "")
    => inlineKeyboardRow.AddLocalizedButton(callTitle, info, CallSirenaCommand.NAME, shortHash);
  public static IInlineKeyboardRow AddCreateButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(createTitle, info, CreateSirenaCommand.NAME);
  public static IInlineKeyboardRow AddDeleteButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string sirenaId, string title = deleteTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, DeleteSirenaCommand.NAME, sirenaId);
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
  public static IInlineKeyboardRow AddMuteButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, User user, SirenRepresentation sirena)
    => inlineKeyboardRow.AddLocalizedButton(muteTitle, info, MuteUserSignalCommand.NAME, user.Id.ToString() + ' ' + sirena.Hash);
  public static IInlineKeyboardRow AddRequestButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
  , string shortHash, string title = requestRightTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, RequestRightsCommand.NAME, shortHash);
  public static IInlineKeyboardRow AddRequestsButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string title = getReuqestsTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, RequestsCommand.NAME);
  public static IInlineKeyboardRow AddRetryButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
  , string command, string title = retryTitle)
  {
    if (LocalizationProvider == null)
      throw new ArgumentNullException(nameof(LocalizationProvider), $"You have to set {nameof(MarkupShortcuts.LocalizationProvider)} field manually before use localized buttons");
    string localTitle = LocalizationProvider.Get(title, info);
    return inlineKeyboardRow.AddCallbackData(localTitle, command);
  }

  public static IInlineKeyboardRow AddSirenaInfoButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
  , string shortHash, string title = getInfoTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, DisplaySirenaInfoCommand.NAME, shortHash);
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
   => inlineKeyboardRow.AddLocalizedButton(subscribeTitle, info, SubscribeCommand.NAME);
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string shortHash)
   => inlineKeyboardRow.AddLocalizedButton(subscribeTitle, info, SubscribeCommand.NAME, shortHash);
  public static IInlineKeyboardRow AddUnsubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, SirenRepresentation siren)
    => inlineKeyboardRow.AddLocalizedButton(unsubscribeTitle, info, UnsubscribeCommand.NAME, siren.Hash);
  public static IInlineKeyboardRow AddDisplayRequestsButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string shortHash, int count)
  {
    if (LocalizationProvider == null)
      throw new ArgumentNullException(nameof(LocalizationProvider), $"You have to set {nameof(MarkupShortcuts.LocalizationProvider)} field manually before use localized buttons");

    string localTitle = LocalizationProvider.Get(getReuqestsTitle, info);
    localTitle = string.Format(localTitle, count);

    return inlineKeyboardRow.AddButton(localTitle, RequestsCommand.NAME, shortHash);
  }

  public static IInlineKeyboardRow AddDisplayResponsiblesButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string sirenaId, int count)
  {
    if (LocalizationProvider == null)
      throw new ArgumentNullException(nameof(LocalizationProvider), $"You have to set {nameof(MarkupShortcuts.LocalizationProvider)} field manually before use localized buttons");
    string localTitle = LocalizationProvider.Get(getResponsiblesTitle, info);
    localTitle = string.Format(localTitle, count);
    return inlineKeyboardRow.AddButton(localTitle, GetResponsiblesListCommand.NAME, sirenaId);
  }

  public static InlineKeyboardMarkup CreateMenuButtonOnlyMarkup(CultureInfo info)
    => new InlineKeyboardMarkup()
    {
      InlineKeyboard = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
          .AddMenuButton(info).EndRow().Build()
    };
  public static InlineKeyboardMarkup ToReplyMarkup(this IInlineKeyboardBuilder builder)
    => new()
    {
      InlineKeyboard = builder.Build()
    };

  static public SendMessage CreateDefaultMessage(ChatId chatId, string message, IReplyMarkup? replyMarkup = null)
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