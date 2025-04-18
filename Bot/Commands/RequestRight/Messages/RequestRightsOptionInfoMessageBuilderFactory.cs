using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestRightsOptionInfoMessageBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenaData, ISendMessageBuilder>
{
  public ISendMessageBuilder Create(IRequestContext context, SirenaData sirena)
  {
    const string notificationLocalizationKey = "command.request_rights.extra_info";
    const string skipButtonLocalizationKey = notificationLocalizationKey + ".skip";

    var info = context.GetCultureInfo();
    string notification = localizationProvider.Get(notificationLocalizationKey, info);
    notification = string.Format(notification, sirena);

    return new OptionalDataRequireMessageBuilder(context, localizationProvider, notification, skipButtonLocalizationKey);
  }
}