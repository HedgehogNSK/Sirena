using RxTelegram.Bot.Interface.BaseTypes.Requests.Base;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public interface IRequestBuilder<out T> where T : BaseRequest
{
  T Build();
}
public interface IBaseRequestBuilder : IRequestBuilder<BaseRequest> { }
public interface ISendMessageBuilder : IRequestBuilder<SendMessage> { }
public interface IEditMessageBuilder : IRequestBuilder<EditMessageText> { }
public interface IEditMessageReplyMarkupBuilder : IRequestBuilder<EditMessageReplyMarkup> { }
public interface IEditMessageMediaBuilder : IRequestBuilder<EditMessageMedia> { }
public interface IEditMessageCaptionBuilder : IRequestBuilder<EditMessageCaption> { }