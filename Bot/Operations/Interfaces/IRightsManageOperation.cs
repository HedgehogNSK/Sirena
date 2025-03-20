namespace Hedgey.Sirena.Bot.Operations;

public interface IRightsManageOperation{
  IObservable<bool> Decline(ulong sirenaId, long requestorId);
}