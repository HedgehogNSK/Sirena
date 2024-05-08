namespace Hedgey.Structure.Plan;

public interface IObservableStep<out TReport>{
  IObservable<TReport> Make();
}
