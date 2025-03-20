using System.Globalization;

namespace Hedgey.Telegram.Messages;

public interface ILocalizedTextGetter : ITextGetter
{
  void Set(CultureInfo info);
}