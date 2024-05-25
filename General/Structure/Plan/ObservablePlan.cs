using System.Reactive.Linq;

namespace Hedgey.Structure.Plan;
public abstract class ObservablePlan<TReport, TSummary>
{
  public IEnumerable<IObservableStep<TReport>> Steps { get; }
  protected IEnumerator<IObservableStep<TReport>>? enumerator = null;
  public IObservableStep<TReport>? CurrentStep { get; protected set; } = default;
  public ObservablePlan(IEnumerable<IObservableStep<TReport>> steps)
  {
    if (steps == null || !steps.Any())
      throw new ArgumentNullException("steps", "Steps couldn't be empty or equal `Null`");
    Steps = steps;
  }
  protected virtual void Init()
  {
    enumerator = Steps.GetEnumerator();
    CurrentStep = default;
  }
  public virtual void Restart()
  {
    Init();
  }

  public virtual IObservable<TSummary> Execute()
  {
    if (enumerator == null)
    {
      Init();

      if (enumerator == null)
        throw new ArgumentNullException("It's a miracle! Enumerator is null. Did you miss the initialization of enumerator?");
    }
    if (CurrentStep == null && !enumerator.MoveNext())
    {
      throw new ArgumentException("No more steps to do");
    }
    return RecursiveCallMake(enumerator).Select(CreateSummary);
  }

  public IObservable<TReport> RecursiveCallMake(IEnumerator<IObservableStep<TReport>> enumerator)
  {
    CurrentStep = enumerator.Current;
    return CurrentStep.Make()
      .SelectMany(_report =>
      {
        Console.WriteLine(CurrentStep.GetType().Name + ' ' + _report);

        if (!IsStepSuccesful(_report))
          return Observable.Return(_report);
        

        if (!enumerator.MoveNext())
        {
#if DEBUG
          Console.WriteLine(GetType().Name + " has no more steps to do!");
#endif
          return Observable.Return(_report);
        }
        return RecursiveCallMake(enumerator);
      });
  }
  protected abstract TSummary CreateSummary(TReport report);
  protected abstract bool IsStepSuccesful(TReport report);
}