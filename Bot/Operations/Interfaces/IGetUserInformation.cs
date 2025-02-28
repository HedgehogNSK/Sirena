using System.Globalization;

namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserInformation{
  IObservable<string> GetNickname(long userID);
  public IObservable<string> GetNickname(long userID, CultureInfo info);
  public IObservable<string> GetNickname(long userID, long chatID, CultureInfo info);
}