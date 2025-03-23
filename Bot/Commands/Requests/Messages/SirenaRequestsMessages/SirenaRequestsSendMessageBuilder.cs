using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class SirenaRequestsSendMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , SirenRepresentation sirena
  , int requestID
  , string userName)
 : SirenaRequestsMessageBuilder(context, localizationProvider, sirena
  , requestID, userName)
  , ISendMessageBuilder
{

  public override SendMessage Build()
  {
    string message = CreateMessage();
    InlineKeyboardMarkup replyMarkup = CreateReplyMarkup();
    return MarkupShortcuts.CreateDefaultMessage(ChatID, message, replyMarkup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context, SirenRepresentation sirena, int requestNumber, string userName)
      => new SirenaRequestsSendMessageBuilder(context, localizationProvider, sirena, requestNumber, userName);
  }
}