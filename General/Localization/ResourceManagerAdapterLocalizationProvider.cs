using System.Globalization;
using System.Resources;

namespace Hedgey.Localization;

public class ResourceManagerAdapterLocalizationProvider : ILocalizationProvider{
  public readonly ResourceManager rm;
  public CultureInfo? CurrentCulture { get; private set; } =null;
  public ResourceManagerAdapterLocalizationProvider(ResourceManager rm, CultureInfo? info =null) { 
    this.rm = rm;
    this.CurrentCulture = info;
  }


  public string Get(string key, CultureInfo info) => rm.GetString(key, info) ?? key;

  public string GetCurrent(string key) => rm.GetString(key,CurrentCulture) ?? key;

  public void SetCulture(CultureInfo info)
  {
    this.CurrentCulture = info;
  }
}