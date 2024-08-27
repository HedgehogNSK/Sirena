using System.Globalization;

namespace Hedgey.Sirena.Bot;

public interface IMessageStrategy
{
  string Get(CultureInfo info, params object[] objects);
}
