using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Utils.Keyboard;
using System.Text;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public abstract class SirenaRequestsMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , SirenRepresentation sirena
  , int requestID
  , string userName)
 : LocalizedBaseRequestBuilder(context.GetTargetChatId(), context.GetCultureInfo(), localizationProvider)
{
  protected readonly SirenRepresentation sirena = sirena;
  protected readonly int requestID = requestID;
  protected readonly string userName = userName;

  protected InlineKeyboardMarkup CreateReplyMarkup()
  {
    var info = context.GetCultureInfo();
    var request = sirena.Requests[requestID];
    var lastRequestId = sirena.Requests.Length - 1;
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    if (lastRequestId != 0)
      keyboardBuilder = keyboardBuilder.AddButton(MarkupShortcuts.Previous, RequestsCommand.NAME
          , sirena.ShortHash + ' ' + (requestID > 0 ? (requestID - 1) : lastRequestId));

    if (context.GetUser().Id == sirena.OwnerId)
    {
      keyboardBuilder = keyboardBuilder.AddDelegateRightsButton(info, sirena
        , request.UID, "command.requests.accept");
      keyboardBuilder = keyboardBuilder.AddDeclineRequestButton(info, sirena, request.UID);
    }

    if (lastRequestId != 0)
      keyboardBuilder.AddButton(MarkupShortcuts.Next, "requests"
        , sirena.ShortHash + ' ' + (requestID < lastRequestId ? (requestID + 1) : 0));
    var replyMarkup = keyboardBuilder.EndRow().ToReplyMarkup();
    return replyMarkup;
  }

  protected string CreateMessage()
  {
    var request = sirena.Requests[requestID];
    const string requestKey = "command.requests.display_by_id";
    StringBuilder builder = new StringBuilder();
    string requestMessage = Localize(requestKey);
    string numeration = sirena.Requests.Length > 1 ? $"[{requestID + 1}/{sirena.Requests.Length}] " : string.Empty;
    builder.AppendFormat(requestMessage, userName, sirena.ShortHash, sirena.Title, numeration);
    if (!string.IsNullOrEmpty(request.Message))
    {
      const string messageKey = "command.requests.display_by_id.user_message";
      requestMessage = Localize(messageKey);
      var userMessage = request.Message.EscapeMarkdownChars();
      builder.Append(requestMessage).Append(userMessage);
    }

    return builder.ToString();
  }
}