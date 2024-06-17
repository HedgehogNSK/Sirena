using Hedgey.Localization;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public abstract class LocalizedMessageBuilder : MessageBuilder
{
  protected LocalizedMessageBuilder(long chatId, CultureInfo info, ILocalizationProvider localizationProvider) : base(chatId)
  {
    Info = info;
    LocalizationProvider = localizationProvider;
  }

  public CultureInfo Info { get; }
  public ILocalizationProvider LocalizationProvider { get; }

  protected string Localize(string key) => LocalizationProvider.Get(key, Info);
}
