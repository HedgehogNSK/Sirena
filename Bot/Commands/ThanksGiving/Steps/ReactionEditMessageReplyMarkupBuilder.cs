using Hedgey.Extensions;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ReactionEditMessageReplyMarkupBuilder(IRequestContext context)
    : IEditMessageReplyMarkupBuilder
{
  private readonly IRequestContext context = context;

  public EditMessageReplyMarkup Build()
  {
    var replyMarkup = context.GetMessage().ReplyMarkup;
    var query = context.GetQuery();
    var reactionButton = FindButton(replyMarkup, query);

    if (reactionButton == null)
      throw new KeyNotFoundException($"{query} not found in callback");

    var emojiCodeString = context.GetArgsString().GetParameterByNumber(1);
    if (!int.TryParse(emojiCodeString, out int emojiCode))
      throw new FormatException($"Can't parse set parameter from \'{nameof(emojiCodeString)}\'");
    var info = context.GetCultureInfo();

    reactionButton.CallbackData = reactionButton.CallbackData.Replace(emojiCodeString, (-emojiCode).ToString());
    reactionButton.Text = MarkupShortcuts.GetEmojiDecription(-emojiCode, info);

    if (emojiCode > 0)
      FindAndRevokeActiveReactions(replyMarkup, reactionButton, info);

    return new EditMessageReplyMarkup()
    {
      ChatId = context.GetTargetChatId(),
      MessageId = context.GetMessage().MessageId,
      ReplyMarkup = replyMarkup,
    };
  }

  private static void FindAndRevokeActiveReactions(InlineKeyboardMarkup replyMarkup
    , InlineKeyboardButton currentButton, System.Globalization.CultureInfo info)
  {
    foreach (var row in replyMarkup.InlineKeyboard)
    {
      foreach (var button in row)
      {
        if (button == currentButton) continue;
        const string commandPrefix = $"/{ReactToSirenaCommand.NAME} ";
        if (!button.CallbackData.StartsWith(commandPrefix)) continue;

        int index = button.CallbackData.IndexOf('-');
        if (index == -1) continue;

        ReadOnlySpan<char> span = button.CallbackData;
        ReadOnlySpan<char> emojiSpan = span[(index + 1)..];
        button.CallbackData = string.Concat(span[..index], emojiSpan);
        var activeEmojiCode = int.Parse(emojiSpan);
        button.Text = MarkupShortcuts.GetEmojiDecription(activeEmojiCode, info);
      }
    }
  }

  protected static InlineKeyboardButton? FindButton(InlineKeyboardMarkup replyMarkup, string query)
    => replyMarkup.InlineKeyboard.SelectMany(_row => _row)
        .FirstOrDefault(_button =>
          _button.CallbackData.Equals(query, StringComparison.Ordinal)
        );
  public class Factory()
   : IFactory<IRequestContext, ReactionEditMessageReplyMarkupBuilder>
  {
    public ReactionEditMessageReplyMarkupBuilder Create(IRequestContext context)
    => new ReactionEditMessageReplyMarkupBuilder(context);
  }
}