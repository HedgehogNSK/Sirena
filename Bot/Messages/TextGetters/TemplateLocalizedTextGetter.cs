using System.Globalization;
using Hedgey.Localization;

namespace Hedgey.Telegram.Messages;

public class TemplateLocalizedTextGetter(ILocalizationProvider provider, CultureInfo info, string key)
: LocalizedTextGetter(provider,info)
{
  public override string Get() => Localize(key);
}