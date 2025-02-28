using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class SirenaHasNoRequestsMessageBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, ISendMessageBuilder>
{
  public ISendMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
  {
    const string localizationKey = "command.requests.error.sirena_has_no_requests";
    var chatID = context.GetTargetChatId();
    var info = context.GetCultureInfo();

    return new PresetLocalizedMessageBuilder(chatID, info, localizationProvider, localizationKey, sirena);
  }
}