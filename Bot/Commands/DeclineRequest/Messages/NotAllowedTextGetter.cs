using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;
public class NotAllowedTextGetter(ILocalizationProvider provider, CultureInfo info, SirenaData sirena)
: SirenaTemplateTextGetter(provider, info, sirena)
{
  protected override string LocalizationKey => "command.decline_request.error.not_owner";
  public sealed class Factory(ILocalizationProvider provider) : IFactory<CultureInfo, SirenaData, NotAllowedTextGetter>
  {
    private readonly ILocalizationProvider provider = provider;

    public NotAllowedTextGetter Create(CultureInfo info, SirenaData sirena)
    => new NotAllowedTextGetter(provider, info, sirena);
  }
}