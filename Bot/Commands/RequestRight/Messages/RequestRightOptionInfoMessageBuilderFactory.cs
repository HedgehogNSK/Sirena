using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class RequestRightOptionInfoMessageBuilderFactory
  : IFactory<IRequestContext, SirenRepresentation, OptionalDataRequireMessageBuilder>
{
  private readonly ILocalizationProvider localizationProvider;

  public RequestRightOptionInfoMessageBuilderFactory(ILocalizationProvider localizationProvider)
  {
    this.localizationProvider = localizationProvider;
  }

  public OptionalDataRequireMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
  {
    var info = context.GetCultureInfo();

    string notificationLocalizationKey = "command.request_rights.extra_info";
    string notification = localizationProvider.Get(notificationLocalizationKey, info);
    notification = string.Format(notification, sirena);

    string skipButtonLocalizationKey = notificationLocalizationKey + ".extra_info.skip";

    return new OptionalDataRequireMessageBuilder(context, localizationProvider, notification, skipButtonLocalizationKey);
  }
}