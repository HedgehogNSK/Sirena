namespace Hedgey.Structure.Plan;

public interface IObservableStep<out TReport>{
  IObservable<TReport> Make();
}
public interface IObservableStep<in TContext,out TReport>{
  IObservable<TReport> Make(TContext context);
}
