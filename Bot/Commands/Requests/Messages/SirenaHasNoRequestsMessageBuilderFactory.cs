using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SirenaHasNoRequestsMessageBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenaData, ISendMessageBuilder>
{
  public ISendMessageBuilder Create(IRequestContext context, SirenaData sirena)
  {
    const string localizationKey = "command.requests.error.sirena_has_no_requests";
    var chatID = context.GetTargetChatId();
    var info = context.GetCultureInfo();

    return new PresetLocalizedMessageBuilder(chatID, info, localizationProvider, localizationKey, sirena);
  }
}