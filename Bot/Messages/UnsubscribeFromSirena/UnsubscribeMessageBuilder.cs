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
    const string successMessage = "You successfully unsubsribed.";
    const string failMessage =  "*Unsubscription failed.* Possible reasons: you're not listener of the Sirena or this sirena doens't exist.";
    if(isSuccess)
    {
      return CreateDefault(successMessage,  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
    }
    else{
      const string unsubscribeTitle = "🔄 Another try";
      var replyMarkup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton(Info)
      .AddCallbackData(unsubscribeTitle, '/'+ UnsubscribeCommand.NAME).EndRow()
      .ToReplyMarkup();
      return CreateDefault(failMessage, replyMarkup);  
    }
  }
}