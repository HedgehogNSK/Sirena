using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SuccesfulDeclineTextGetter(ILocalizationProvider provider, CultureInfo info, SirenaData sirena, object param)
: SirenaWithExtrasTemplateTextGetter(provider, info, sirena, param)
{
  protected override string LocalizationKey => "command.decline_request.success";
  public sealed class Factory(ILocalizationProvider provider) : IFactory<CultureInfo, SirenaData, object, SuccesfulDeclineTextGetter>
  {
    private readonly ILocalizationProvider provider = provider;

    public SuccesfulDeclineTextGetter Create(CultureInfo info, SirenaData sirena, object param)
    => new SuccesfulDeclineTextGetter(provider, info, sirena, param);
  }
}