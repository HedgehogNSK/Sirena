using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class StringNotIdMessageBuilder : MessageBuilder
{
  private string param;

  public StringNotIdMessageBuilder(long chatId, string param)
  :base(chatId)
  {
    this.chatId = chatId;
    this.param = param;
  }

  public override SendMessage Build()
  {
    const string notification = "Incorrect parameter! Please provide Sirena ID to call. It has to be ID of a Sirena that you own or are responsible for";
    return CreateDefault(notification, MarkupShortcuts.CreateMenuButtonOnlyMarkup());
  }
}
