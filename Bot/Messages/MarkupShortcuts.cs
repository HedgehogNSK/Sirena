using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Utils.Keyboard;
using RxTelegram.Bot.Utils.Keyboard.Interfaces;

namespace Hedgey.Sirena.Bot;

public static class MarkupShortcuts
{
  const string menuTitle = "🧾 Menu";
  const string findTitle = "🔎 Find";
  const string subscribeTitle = "🔔 Subscribe";
  private static IInlineKeyboardRow AddButton(this IInlineKeyboardRow inlineKeyboardRow
    ,string title, string commandName)
  =>  inlineKeyboardRow.AddCallbackData(title, '/' + commandName);
    public static IInlineKeyboardRow AddMenuButton(this IInlineKeyboardRow inlineKeyboardRow)
    => inlineKeyboardRow.AddButton(menuTitle, MenuBotCommand.NAME);
  public static IInlineKeyboardRow AddFindButton(this IInlineKeyboardRow inlineKeyboardRow)
    => inlineKeyboardRow.AddButton(findTitle, FindSirenaCommand.NAME);
  public static IInlineKeyboardRow AddSubscribeButton(this IInlineKeyboardRow inlineKeyboardRow)
    => inlineKeyboardRow.AddButton(subscribeTitle, SubscribeCommand.NAME);
  public static IReplyMarkup CreateMenuButtonOnlyMarkup()
  {
    return new InlineKeyboardMarkup()
    {
      InlineKeyboard = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddMenuButton().EndRow().Build()
    };
  }
  public static IReplyMarkup ToMarkup(this IInlineKeyboardBuilder builder){
    return new InlineKeyboardMarkup()
    {
      InlineKeyboard = builder.Build()
    };
  }

}