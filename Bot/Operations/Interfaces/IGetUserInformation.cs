using System.Globalization;

namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserInformation{
  IObservable<string> GetNickname(long userId);
  IObservable<string> GetNickname(long userId, CultureInfo info);
  IObservable<string> GetNickname(long userId, long chatId, CultureInfo info);
}