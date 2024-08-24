using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class RequestRightsOptionInfoMessageBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>
{
  public IMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
  {
    const string notificationLocalizationKey = "command.request_rights.extra_info";
    const string skipButtonLocalizationKey = notificationLocalizationKey + ".skip";

    var info = context.GetCultureInfo();
    string notification = localizationProvider.Get(notificationLocalizationKey, info);
    notification = string.Format(notification, sirena);

    return new OptionalDataRequireMessageBuilder(context, localizationProvider, notification, skipButtonLocalizationKey);
  }
}