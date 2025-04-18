using Hedgey.Localization;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

sealed public class SwitchButtonCommandReplyMarkupBuilder : IEditMessageReplyMarkupBuilder
{
  private readonly ILocalizationProvider localizationProvider;
  private readonly IRequestContext context;
  private readonly Option replacement;

  public SwitchButtonCommandReplyMarkupBuilder(ILocalizationProvider localizationProvider
    , Option replacement, IRequestContext context)
  {
    this.localizationProvider = localizationProvider;
    this.context = context;
    this.replacement = replacement;
  }

  public EditMessageReplyMarkup Build()
  {
    var message = context.GetMessage();
    var replyMarkup = message.ReplyMarkup;
    var button = FindCallbackButton(context);
    button.CallbackData = ReplaceCommandName(button.CallbackData, context.GetCommandName(), replacement.commandName);
    button.Text = localizationProvider.Get(replacement.titleKey, context.GetCultureInfo());
    return new EditMessageReplyMarkup()
    {
      ChatId = context.GetTargetChatId(),
      MessageId = message.MessageId,
      ReplyMarkup = replyMarkup,
    };
  }
  public static InlineKeyboardButton FindCallbackButton(IRequestContext context)
  {
    InlineKeyboardMarkup replyMarkup = context.GetMessage().ReplyMarkup;
    var query = context.GetQuery();
    return replyMarkup.InlineKeyboard.SelectMany(_row => _row)
          .First(_button =>
            string.Equals(_button.CallbackData, query, StringComparison.Ordinal)
          );
  }
  private static string ReplaceCommandName(string input, string oldCommand, string newCommand)
  {
    ReadOnlySpan<char> span = input.AsSpan();

    if (span.Length == 0 || span[0] != '/')
      return input;

    int spaceIndex = span.IndexOf(' ');
    if (spaceIndex == -1)
      spaceIndex = span.Length;

    var commandSpan = span.Slice(1, spaceIndex - 1);

    if (!commandSpan.SequenceEqual(oldCommand.AsSpan()))
      return input;

    return string.Concat(
        "/",
        newCommand,
        span.Slice(spaceIndex).ToString()
    );
  }
  public record struct Option(string commandName, string titleKey)
  {
    public readonly string commandName = commandName;
    public readonly string titleKey = titleKey;
  }

  public class Factory(ILocalizationProvider provider, Option option) : IFactory<IRequestContext, SwitchButtonCommandReplyMarkupBuilder>
  {
    public SwitchButtonCommandReplyMarkupBuilder Create(IRequestContext context)
      => new SwitchButtonCommandReplyMarkupBuilder(provider, option, context);
  }
}