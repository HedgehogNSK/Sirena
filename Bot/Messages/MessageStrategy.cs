using Hedgey.Localization;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class MessageStrategy(ILocalizationProvider provider, string key)
: IMessageStrategy
{
  public string Get(CultureInfo info, params object[] objects)
  {
    string message = provider.Get(key, info);
    if (objects.Length != 0)
      message = string.Format(message, objects);
    return message;
  }
}