using Hedgey.Localization;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class SirenaRequestsSendMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider, RequestsCommand.RequestInfo requestInfo)
 : SirenaRequestsMessageBuilder(context, localizationProvider, requestInfo)
  , ISendMessageBuilder
{

  public override SendMessage Build()
  {
    string message = CreateMessage();
    InlineKeyboardMarkup replyMarkup = CreateReplyMarkup();
    return MarkupShortcuts.CreateDefaultMessage(ChatID, message, replyMarkup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, RequestsCommand.RequestInfo, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context, RequestsCommand.RequestInfo info)
      => new SirenaRequestsSendMessageBuilder(context, localizationProvider, info);
  }
}