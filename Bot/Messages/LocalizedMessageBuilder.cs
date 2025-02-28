using Hedgey.Localization;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public abstract class LocalizedBaseRequestBuilder : BaseRequestBuilder
{
  protected LocalizedBaseRequestBuilder(long chatId, CultureInfo info, ILocalizationProvider localizationProvider) : base(chatId)
  {
    Info = info;
    LocalizationProvider = localizationProvider;
  }

  public virtual LocalizedBaseRequestBuilder  SetCulture(CultureInfo info)
  {
    Info = info;
    return this;
  }
  public CultureInfo Info { get; private set; }
  public ILocalizationProvider LocalizationProvider { get; }

  protected string Localize(string key) => LocalizationProvider.Get(key, Info);
}