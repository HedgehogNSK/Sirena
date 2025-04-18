using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NoRequestorTextGetter(ILocalizationProvider provider, CultureInfo info, SirenaData sirena, object param)
: SirenaWithExtrasTemplateTextGetter(provider, info, sirena, param)
{
  protected override string LocalizationKey => "command.decline_request.error.no_requestor";

  public sealed class Factory(ILocalizationProvider provider) : IFactory<CultureInfo, SirenaData, object, NoRequestorTextGetter>
  {
    private readonly ILocalizationProvider provider = provider;

    public NoRequestorTextGetter Create(CultureInfo info, SirenaData sirena, object param)
    => new NoRequestorTextGetter(provider, info, sirena, param);
  }
}