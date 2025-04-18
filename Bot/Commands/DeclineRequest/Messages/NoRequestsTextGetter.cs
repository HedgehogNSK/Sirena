using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NoRequestsTextGetter(ILocalizationProvider provider, CultureInfo info, SirenaData sirena)
: SirenaTemplateTextGetter(provider, info, sirena)
{
  protected override string LocalizationKey => "command.decline_request.error.no_requests";
  public sealed class Factory(ILocalizationProvider provider) : IFactory<CultureInfo, SirenaData, NoRequestsTextGetter>
  {
    private readonly ILocalizationProvider provider = provider;

    public NoRequestsTextGetter Create(CultureInfo info, SirenaData param)
    => new NoRequestsTextGetter(provider, info, param);
  }
}