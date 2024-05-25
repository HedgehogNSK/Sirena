using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

internal class ExtraInformationMessageBuilder : MessageBuilder
{
  private SirenRepresentation sirena;

  public ExtraInformationMessageBuilder(long chatId, SirenRepresentation sirena) : base(chatId)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    const string notification = "You can send extra information for subscribers or just click the *Call* button.";
    
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddCallSirenaButton().AddMenuButton().EndRow()
      .ToReplyMarkup();

    return CreateDefault(notification,markup);
  }
}