using System.Globalization;
using Hedgey.Localization;
using Hedgey.Sirena;
using Hedgey.Sirena.Entities;

namespace Hedgey.Telegram.Messages;

public abstract class SirenaTemplateTextGetter(ILocalizationProvider provider, CultureInfo info, SirenaData sirena)
: LocalizedTextGetter(provider, info)
{
  protected abstract string LocalizationKey { get; }
  private SirenaData sirena = sirena;
  public override string Get()
  {
    if (sirena == null)
      throw new ArgumentNotInitializedException(nameof(sirena));
    var message = Localize(LocalizationKey);
    message = string.Format(message, sirena);
    return message;
  }
}

public abstract class SirenaWithExtrasTemplateTextGetter(ILocalizationProvider provider
, CultureInfo info
  , SirenaData sirena
  , object extraInfo)
: LocalizedTextGetter(provider, info)
{
  protected abstract string LocalizationKey { get; }
  private readonly SirenaData sirena = sirena;
  private readonly object info = extraInfo;
  public override string Get()
  {
    var message = Localize(LocalizationKey);
    message = string.Format(message, sirena, info);
    return message;
  }
}