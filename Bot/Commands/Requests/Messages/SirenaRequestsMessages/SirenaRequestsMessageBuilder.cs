using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Utils.Keyboard;
using System.Text;

namespace Hedgey.Sirena.Bot;
public abstract class SirenaRequestsMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , RequestsCommand.RequestInfo requestInfo)
 : LocalizedBaseRequestBuilder(context.GetTargetChatId(), context.GetCultureInfo(), localizationProvider)
{
  private readonly RequestsCommand.RequestInfo requestInfo = requestInfo;

  protected InlineKeyboardMarkup CreateReplyMarkup()
  {
    var info = context.GetCultureInfo();
    var sirena = requestInfo.Sirena;
    var requestID = requestInfo.RequestID;
    var requestorUID = requestInfo.RequestorID;
    var lastRequestId = sirena.Requests.Length - 1;
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    if (lastRequestId != 0)
      keyboardBuilder = keyboardBuilder.AddCallbackButton(MarkupShortcuts.Previous, RequestsCommand.NAME
          , sirena.ShortHash + ' ' + (requestID > 0 ? (requestID - 1) : lastRequestId));

    if (context.GetUser().Id == sirena.OwnerId)
    {
      keyboardBuilder = keyboardBuilder.AddDelegateRightsButton(info, sirena
        , requestorUID, "command.requests.accept");
      keyboardBuilder = keyboardBuilder.AddDeclineRequestButton(info, sirena, requestorUID);
    }

    if (lastRequestId != 0)
      keyboardBuilder.AddCallbackButton(MarkupShortcuts.Next, "requests"
        , sirena.ShortHash + ' ' + (requestID < lastRequestId ? (requestID + 1) : 0));
    var replyMarkup = keyboardBuilder.EndRow().ToReplyMarkup();
    return replyMarkup;
  }

  protected string CreateMessage()
  {
    var sirena = requestInfo.Sirena;
    var requestID = requestInfo.RequestID;
    var request = sirena.Requests[requestID];
    const string requestKey = "command.requests.display_by_id";
    StringBuilder builder = new StringBuilder();
    string requestMessage = Localize(requestKey);
    string numeration = sirena.Requests.Length > 1 ? $"[{requestID + 1}/{sirena.Requests.Length}] " : string.Empty;
    builder.AppendFormat(requestMessage, requestInfo.Username, sirena.ShortHash, sirena.Title, numeration);
    if (!string.IsNullOrEmpty(request.Message))
    {
      const string messageKey = "command.requests.display_by_id.user_message";
      requestMessage = Localize(messageKey);
      var userMessage = request.Message.EscapeMarkdownChars();
      builder.AppendLine().Append(requestMessage).Append(userMessage);
    }

    return builder.ToString();
  }
}