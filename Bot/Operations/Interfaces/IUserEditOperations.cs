namespace Hedgey.Sirena.Bot.Operations;

public interface IUserEditOperations
{
  IObservable<UpdateState> CreateUser(long userID, long chatID);
}