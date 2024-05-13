namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserInformation{
  IObservable<string> GetNickname(long uid);
}