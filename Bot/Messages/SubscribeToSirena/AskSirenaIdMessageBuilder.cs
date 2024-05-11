using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class AskSirenaIdMessageBuilder : MessageBuilder
{
  public AskSirenaIdMessageBuilder(long chatId)
: base(chatId) { }

  public override SendMessage Build()
  {
    const string errorDescription = "*Please input Sirena ID*.\nYou have to input exact ID of Sirena that you are going to subscribe to.\n\nYou can use *Find* command to find a Sirena by title.";
    
    var markup = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow().AddFindButton().AddMenuButton().EndRow()
        .ToMarkup();

    return CreateDefault(errorDescription, markup);
  }
}