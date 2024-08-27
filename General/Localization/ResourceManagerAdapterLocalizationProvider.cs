using System.Globalization;
using System.Resources;

namespace Hedgey.Localization;

public class ResourceManagerAdapterLocalizationProvider : ILocalizationProvider
{
  public readonly ResourceManager rm;
  public CultureInfo? CurrentCulture { get; private set; } = null;
  public ResourceManagerAdapterLocalizationProvider(ResourceManager rm)
  {
    this.rm = rm;
  }

  public string Get(string key, CultureInfo info)
  {
    string? result = rm.GetString(key, info);
    if (result == null)
    {
      result = key;
      Console.WriteLine($"Localization key: {key} has no pair");
    }
    return result;
  }

  public string GetCurrent(string key) => rm.GetString(key, CurrentCulture) ?? key;

  public void SetCulture(CultureInfo info)
  {
    this.CurrentCulture = info;
  }
}