using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

internal class UnsubscribeMessageBuilder : LocalizedMessageBuilder
{
  private bool isSuccess;

  public UnsubscribeMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, bool isSuccess) : base(chatId,info,localizationProvider)
  {
    this.isSuccess = isSuccess;
  }
  public override SendMessage Build()
  {
     string successMessage = Localize("command.unsubscribe.success");
     string failMessage =  Localize( "command.unsubscribe.fail");
    if(isSuccess)
    {
      return CreateDefault(successMessage,  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
    }
    else{
       string unsubscribeTitle =  Localize("menu.buttons.anotherTry.title");
      var replyMarkup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton(Info)
      .AddCallbackData(unsubscribeTitle, '/'+ UnsubscribeCommand.NAME).EndRow()
      .ToReplyMarkup();
      return CreateDefault(failMessage, replyMarkup);  
    }
  }
}