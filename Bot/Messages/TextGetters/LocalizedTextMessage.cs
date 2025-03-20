using Hedgey.Localization;
using System.Globalization;

namespace Hedgey.Telegram.Messages;

public abstract class LocalizedTextGetter(ILocalizationProvider provider, CultureInfo info) 
: ILocalizedTextGetter
{
  private readonly ILocalizationProvider provider = provider;
  private CultureInfo info = info;

  public abstract string Get();

  public void Set(CultureInfo info)
  {
    this.info = info;
  }
  protected virtual string Localize(string key)
    => provider.Get(key, info);
}