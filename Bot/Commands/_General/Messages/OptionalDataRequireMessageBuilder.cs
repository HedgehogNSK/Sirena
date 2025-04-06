using Hedgey.Localization;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class OptionalDataRequireMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , string notificationMessage
  , string skipButtonLocalizationKey)
   : MessageBuilder(context.GetChat().Id, context.GetCultureInfo(), localizationProvider)
{
  protected string SkipButtonCallback => '/' + context.GetCommandName();

  public override SendMessage Build()
  {
    string notification = notificationMessage;
    string skipTitle = Localize(skipButtonLocalizationKey);

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
        .AddCallbackData(skipTitle, SkipButtonCallback)
        .AddMenuButton(Info).EndRow().ToReplyMarkup();

    return CreateDefault(notification, markup);
  }
}