using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

namespace Hedgey.Sirena.Bot;

public class OptionalDataRequireMessageBuilder(IRequestContext context
  , ILocalizationProvider localizationProvider
  , string notificationLocalizationKey
  , string skipButtonLocalizationKey)
   : LocalizedMessageBuilder(context.GetChat().Id, context.GetCultureInfo(), localizationProvider)
{
  protected string SkipButtonCallback => '/'+context.GetCommandName();

  public override SendMessage Build()
  {
    string notification = Localize(notificationLocalizationKey);
    string skipTitle = Localize(skipButtonLocalizationKey);

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
        .AddCallbackData(skipTitle, SkipButtonCallback)
        .AddMenuButton(Info).EndRow().ToReplyMarkup();

    return CreateDefault(notification, markup);
  }
}