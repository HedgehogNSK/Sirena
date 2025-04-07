using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
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
  public const string declineRequestTitle = prefix + "decline.title";
  public const string delegateTitle = prefix + "delegate.title";
  public const string deleteTitle = prefix + "delete.title";
  public const string displaySirenasTitle = prefix + "display_sirenas.title";
  public const string getReuqestsTitle = prefix + "requests.title";
  public const string getResponsiblesTitle = prefix + "get_responsibles.title";
  public const string reactionTitle = prefix + "reaction.{0}";
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
    string localTitle = LocalizationProvider?.Get(textKey, info)
      ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));
    return inlineKeyboardRow.AddButton(localTitle, commandName, param);
  }

  public static IInlineKeyboardRow AddMenuButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(menuTitle, info, MenuBotCommand.NAME);
  public static IInlineKeyboardRow AddCallSirenaButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string shortHash = "")
    => inlineKeyboardRow.AddLocalizedButton(callTitle, info, CallSirenaCommand.NAME, shortHash);
  public static IInlineKeyboardRow AddCreateButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info)
    => inlineKeyboardRow.AddLocalizedButton(createTitle, info, CreateSirenaCommand.NAME);
  public static IInlineKeyboardRow AddDeclineRequestButton(this IInlineKeyboardRow inlineKeyboardRow
  , CultureInfo info, SirenRepresentation sirena, long userId, string title = declineRequestTitle)
  => inlineKeyboardRow.AddLocalizedButton(title, info, DeclineRequestCommand.NAME
      , sirena.ShortHash + ' ' + userId);
  public static IInlineKeyboardRow AddDelegateRightsButton(this IInlineKeyboardRow inlineKeyboardRow
  , CultureInfo info, SirenRepresentation sirena, long userId, string title = delegateTitle)
  => inlineKeyboardRow.AddLocalizedButton(title, info, DelegateRightsCommand.NAME
      , sirena.ShortHash + ' ' + userId);

  public static IInlineKeyboardRow AddDeleteButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string sirenaId, string title = deleteTitle)
    => inlineKeyboardRow.AddLocalizedButton(title, info, DeleteSirenaCommand.NAME, sirenaId);
  public static IInlineKeyboardRow AddDisplaySubscriptionsButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, int count = 0, string title = subscriptionsTitle)
  {
    string localTitle = LocalizationProvider?.Get(title, info)
      ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));
    if (count != 0)
      _ = $" [{count}]";
    return inlineKeyboardRow.AddButton(localTitle, GetSubscriptionsListCommand.NAME);
  }

  public static IInlineKeyboardRow AddDisplayUserSirenasButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
    , int count = 0, string title = displaySirenasTitle)
  {
    string localTitle = LocalizationProvider?.Get(title, info)
      ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));
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
  public static IInlineKeyboardRow AddReactButton(this IInlineKeyboardRow inlineKeyboardRow
    , CultureInfo info, ObjectId callId, int reaction)
  {
    string param = $"{callId} {reaction}";
    string title = string.Format(reactionTitle, reaction);
    return inlineKeyboardRow.AddLocalizedButton(title, info, ReactToSirenaCommand.NAME, param);
  }
  public static IInlineKeyboardRow AddRequestsButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
    , int count, string shortHash = "")
  {
    string localTitle = LocalizationProvider?.Get(getReuqestsTitle, info)
     ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));
    localTitle = string.Format(localTitle, count);

    return inlineKeyboardRow.AddButton(localTitle, RequestsCommand.NAME, shortHash);
  }

  public static IInlineKeyboardRow AddRetryButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info
  , string command, string title = retryTitle)
  {
    string localTitle = LocalizationProvider?.Get(title, info)
      ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));
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

  public static IInlineKeyboardRow AddDisplayResponsiblesButton(this IInlineKeyboardRow inlineKeyboardRow, CultureInfo info, string sirenaId, int count)
  {
    string localTitle = LocalizationProvider?.Get(getResponsiblesTitle, info)
     ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));

    localTitle = string.Format(localTitle, count);
    return inlineKeyboardRow.AddButton(localTitle, GetResponsiblesListCommand.NAME, sirenaId);
  }

  public static InlineKeyboardMarkup CreateMenuButtonOnlyMarkup(CultureInfo info)
    => new InlineKeyboardMarkup()
    {
      InlineKeyboard = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
          .AddMenuButton(info).EndRow().Build()
    };

  public static string GetEmojiDecription(int emojiCode, CultureInfo info)
  {
    string key = $"{prefix}reaction.{Math.Abs(emojiCode)}{(emojiCode < 0 ? ".unset" : string.Empty)}";
    return LocalizationProvider?.Get(key, info)
      ?? throw new ArgumentNotInitializedException(nameof(LocalizationProvider));
  }
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