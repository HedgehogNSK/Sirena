namespace Hedgey.Structure.Plan;

public interface IObservablePlan<TReport>{
 IObservable<TReport> Execute();
}

public interface IObservablePlan<TReport,TContext>{
 IObservable<TReport> Execute(TContext  context);
}
