using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Utils.Keyboard;
using RxTelegram.Bot.Utils.Keyboard.Interfaces;

namespace Hedgey.Sirena.Bot;

public static class MarkupShortcuts
{
  public const string menuTitle = "🧾 Menu";
  public const string callTitle = "🔊 Launch Sirena";
  public const string createTitle = "🆕 Create";
  public const string getInfoTitle = "ℹ️ Info";
  public const string findTitle = "🔎 Find";
  public const string subscribeTitle = "🔔 Subscribe";
  public const string unsubscribeTitle = "🔕 UnSubscribe";
  public const string deleteTitle = "🗑 Delete";
  public const string displaySirenasTitle = "🖥 Your Sirenas";
  public const string getReuqestsTitle = "👽 Requests [{0}]";
  public const string getResponsiblesTitle = "🫡 Responsibles [{0}]";
  public const string requestRightTitle = "🙏 Ask rights";
  public const string subscriptionsTitle = "👀 Subscriptions";

  private static IInlineKeyboardRow AddButton(this IInlineKeyboardRow inlineKeyboardRow
    , string title, string commandName)
  {
    string command = '/' + commandName;
    return inlineKeyboardRow.AddCallbackData(title, command);
  }
  private static IInlineKeyboardRow AddButton(this IInlineKeyboardRow inlineKeyboardRow
    , string title, string commandName, string param = "")
  {
    string command = '/' + commandName;
    if (!string.IsNullOrEmpty(param))
      command += ' ' + param;
    return inlineKeyboardRow.AddCallbackData(title, command);
  }

  public static IInlineKeyboardRow AddMenuButton(this IInlineKeyboardRow inlineKeyboardRow)
    => inlineKeyboardRow.AddButton(menuTitle, MenuBotCommand.NAME);
  public static IInlineKeyboardRow AddCallSirenaButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId)
    => inlineKeyboardRow.AddButton(callTitle, CallSirenaCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddCreateButton(this IInlineKeyboardRow inlineKeyboardRow)
    => inlineKeyboardRow.AddButton(createTitle, CreateSirenaCommand.NAME);
  public static IInlineKeyboardRow AddDeleteButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId, string title = deleteTitle)
    => inlineKeyboardRow.AddButton(title, DeleteSirenaCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddDisplaySubscriptionsButton(this IInlineKeyboardRow inlineKeyboardRow, int count = 0, string title = subscriptionsTitle)
  {
    if (count != 0)
      title += $" [{count}]";
    return inlineKeyboardRow.AddButton(title, GetSubscriptionsListCommand.NAME);
  }

  public static IInlineKeyboardRow AddDisplayUserSirenasButton(this IInlineKeyboardRow inlineKeyboardRow
    , int count = 0, string title = displaySirenasTitle)
  {
    if (count != 0)
      title += $" [{count}]";
    return inlineKeyboardRow.AddButton(title, DisplayUsersSirenasCommand.NAME);
  }

  public static IInlineKeyboardRow AddFindButton(this IInlineKeyboardRow inlineKeyboardRow)
    => inlineKeyboardRow.AddButton(findTitle, FindSirenaCommand.NAME);
  public static IInlineKeyboardRow AddRequestButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId, string title = requestRightTitle) => inlineKeyboardRow.AddButton(title, RequestRightsCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddSirenaInfoButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId, string title = getInfoTitle) => inlineKeyboardRow.AddButton(title, DisplaySirenaInfoCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow)
   => inlineKeyboardRow.AddButton(subscribeTitle, SubscribeCommand.NAME);
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId)
   => inlineKeyboardRow.AddButton(subscribeTitle, SubscribeCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddUnsubscribeButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId)
    => inlineKeyboardRow.AddButton(unsubscribeTitle, UnsubscribeCommand.NAME, sirenaId.ToString());
  public static IInlineKeyboardRow AddDisplayRequestsButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId, int count)
  {
    string title = string.Format(getReuqestsTitle, count);
    return inlineKeyboardRow.AddButton(title, GetRequestsListCommand.NAME, sirenaId.ToString());
  }

  public static IInlineKeyboardRow AddDisplayResponsiblesButton(this IInlineKeyboardRow inlineKeyboardRow, ObjectId sirenaId, int count)
  {
    string title = string.Format(getResponsiblesTitle, count);
    return inlineKeyboardRow.AddButton(title, GetResponsiblesListCommand.NAME, sirenaId.ToString());
  }

  public static IReplyMarkup CreateMenuButtonOnlyMarkup()
  {
    return new InlineKeyboardMarkup()
    {
      InlineKeyboard = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddMenuButton().EndRow().Build()
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