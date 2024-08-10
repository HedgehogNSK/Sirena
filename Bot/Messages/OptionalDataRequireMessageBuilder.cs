using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class OptionalDataRequireMessageBuilder : LocalizedMessageBuilder
{
  private readonly IRequestContext context;
  private readonly string messageKey;
  private readonly string skipKey;

  public OptionalDataRequireMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , string notificationLocalizationKey
  , string skipButtonLocalizationKey)
  : base(context.GetChat().Id, context.GetCultureInfo(), localizationProvider)
  {
    this.messageKey = notificationLocalizationKey;
    this.skipKey = skipButtonLocalizationKey;
    this.context = context;
  }
  protected string SkipButtonCallback => context.GetMessage().Text;

  public override SendMessage Build()
  {
    string notification = Localize(messageKey);
    string skipTitle = Localize(skipKey);

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
        .AddCallbackData(skipTitle, SkipButtonCallback)
        .AddMenuButton(Info).EndRow().ToReplyMarkup();

    return CreateDefault(notification, markup);
  }
}