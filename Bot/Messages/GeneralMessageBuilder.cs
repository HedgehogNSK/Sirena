using Hedgey.Sirena;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Telegram.Messages;
public class GeneralMessageBuilder : SendMessageBuilder<GeneralMessageBuilder>
{
  private ITextGetter? messageCreator = null;
  private IReplyMarkupGetter? replyMarkupCreator = null;
  public GeneralMessageBuilder Set(ITextGetter messageCreator)
  {
    this.messageCreator = messageCreator;
    return this;
  }
  public GeneralMessageBuilder Set(IRequestContext context)
    => Set(context.GetTargetChatId());
  public GeneralMessageBuilder Set(IReplyMarkupGetter replyMarkupCreator)
  {
    this.replyMarkupCreator = replyMarkupCreator;
    return this;
  }
  public override SendMessage Build()
  {
    var message = messageCreator?.Get() ?? string.Empty;
    var replyMarkup = replyMarkupCreator?.Get();
    return CreateDefault(message, replyMarkup);
  }
}
