namespace Hedgey.Sirena.Bot.Operations;

public interface IRightsRequestOperation{
  IObservable<Result> Send(ulong sirenaId, long requestorId, string message);

  public record Result(bool isSirenaFound, bool isSuccess);
}