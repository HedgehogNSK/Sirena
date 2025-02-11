namespace Hedgey.Sirena.Bot.Operations;

public interface IUserInfoOperations
{
  IObservable<UserStatistics> Get(long uid);
}