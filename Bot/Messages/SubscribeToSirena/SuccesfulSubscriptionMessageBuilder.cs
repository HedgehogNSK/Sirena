using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class SuccesfulSubscriptionMessageBuilder : MessageBuilder
{
  private SirenRepresentation representation;

  public SuccesfulSubscriptionMessageBuilder(long chatId
    , SirenRepresentation representation)
   : base(chatId)
  {
    this.representation = representation;
  }

  public override SendMessage Build()
  {
    IReplyMarkup markup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton().EndRow()
      .ToReplyMarkup();

    const string notificationText = "You successfully subscribed to *Sirena*: _{0}_";
    var message = string.Format(notificationText, representation.Title);
    return CreateDefault(message, markup);
  }
}
