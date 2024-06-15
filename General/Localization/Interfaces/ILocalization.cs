using System.Globalization;

namespace Hedgey.Localization;

public interface ILocalizationProvider{
  void SetCulture(CultureInfo info);
  string GetCurrent(string key);
  string Get(string key,CultureInfo cultureInfo);
}