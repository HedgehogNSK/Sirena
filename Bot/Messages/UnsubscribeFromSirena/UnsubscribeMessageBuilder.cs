using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

internal class UnsubscribeMessageBuilder : MessageBuilder
{
  private bool isSuccess;

  public UnsubscribeMessageBuilder(long chatId, bool isSuccess) : base(chatId)
  {
    this.isSuccess = isSuccess;
  }
  public override SendMessage Build()
  {
    const string successMessage = "You successfully unsubsribed.";
    const string failMessage =  "*Unsubscription failed.* Possible reasons: you're not listener of the Sirena or this sirena doens't exist.";
    if(isSuccess)
    {
      return CreateDefault(successMessage, MarkupShortcuts.CreateMenuButtonOnlyMarkup());
    }
    else{
      const string unsubscribeTitle = "🔄 Another try";
      var replyMarkup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton()
      .AddCallbackData(unsubscribeTitle, '/'+ UnsubscribeCommand.NAME).EndRow()
      .ToReplyMarkup();
      return CreateDefault(failMessage, replyMarkup);  
    }
  }
}