using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Enums;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class SirenaRequestsEditMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , SirenRepresentation sirena
  , int requestNumber
  , string userName)
 : SirenaRequestsMessageBuilder(context, localizationProvider, sirena
  , requestNumber, userName)
  , IEditMessageBuilder
{
  private readonly IRequestContext context = context;

  public override EditMessageText Build()
  {
    string message = CreateMessage();
    InlineKeyboardMarkup replyMarkup = CreateReplyMarkup();
    var contextMessage = context.GetMessage();
    return new EditMessageText()
    {
      ChatId = context.GetTargetChatId(),
      MessageId = contextMessage.MessageId,
      ReplyMarkup = replyMarkup,
      Text = message,
      ParseMode = ParseMode.Markdown,
    };
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, int, string, IEditMessageBuilder>
  {
    public IEditMessageBuilder Create(IRequestContext context, SirenRepresentation sirena, int requestNumber, string userName)
      => new SirenaRequestsEditMessageBuilder(context, localizationProvider, sirena, requestNumber, userName);
  }
}