using Hedgey.Localization;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class NoRequestsMessageBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, ISendMessageBuilder>
{
  public ISendMessageBuilder Create(IRequestContext context)
  {
    const string localizationKey = "command.requests.error.no_pending_requests";
    var chatID = context.GetTargetChatId();
    var info = context.GetCultureInfo();

    return new PresetLocalizedMessageBuilder(chatID, info, localizationProvider, localizationKey);
  }
}