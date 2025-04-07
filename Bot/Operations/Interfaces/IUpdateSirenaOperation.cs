namespace Hedgey.Sirena.Bot.Operations;

public interface IUpdateSirenaOperation{
  IObservable<bool> UpdateDefault(ulong sirenaId);
}