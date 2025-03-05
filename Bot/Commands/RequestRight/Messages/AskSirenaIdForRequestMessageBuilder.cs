using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class AskSirenaIdForRequestMessageBuilder : MessageBuilder
{
  public AskSirenaIdForRequestMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider)
: base(chatId, info, localizationProvider) { }

  public override SendMessage Build()
  {
    const string messageKey = "command.request.askSirenaId";
    string message = Localize(messageKey);

    var markup = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow().AddDisplaySubscriptionsButton(Info).AddMenuButton(Info).EndRow()
        .ToReplyMarkup();

    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, ISendMessageBuilder>
  {

    public ISendMessageBuilder Create(IRequestContext context)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      return new AskSirenaIdForRequestMessageBuilder(chatId, info, localizationProvider);
    }
  }
}