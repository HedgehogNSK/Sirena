using System.Reactive.Linq;

namespace Hedgey.Structure.Plan;
public abstract class ObservablePlan<TReport>
{
  public IEnumerable<IObservableStep<TReport>> Steps { get; }
  protected IEnumerator<IObservableStep<TReport>>? enumerator = null;
  public IObservableStep<TReport>? CurrentStep { get; protected set; } = default;
  public bool IsComplete { get; protected set; } = false;
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
    IsComplete = false;
  }
  public virtual void Restart()
  {
    Init();
  }
  public virtual IObservable<TReport> Execute()
  {
    if (enumerator == null)
    {
      Init();

      if (enumerator == null)
        throw new ArgumentNullException("It's a miracle! Enumerator is null. Did you miss the initialization of enumerator?");
    }
    if (CurrentStep == null && !enumerator.MoveNext())
    {
      throw new ArgumentException("No steps to do");
    }
    CurrentStep = enumerator.Current;
    return CurrentStep.Make()
      .Expand(_report =>
      {
        bool nextIsSet = false;
        if (IsStepSuccesful(_report))
        {
          nextIsSet = enumerator.MoveNext();
          if (nextIsSet)
          {
            CurrentStep = enumerator.Current;
            return CurrentStep.Make();
          }
        }
        if (!nextIsSet)
          IsComplete = true;
        return Observable.Empty<TReport>();
      })
      .Do(_report => Console.WriteLine(CurrentStep.GetType().Name + ": " + _report));
  }
  protected abstract bool IsStepSuccesful(TReport report);
}

public abstract class ObservablePlan<TContext, TReport>
{
  public IEnumerable<IObservableStep<TContext, TReport>> Steps { get; }
  protected IEnumerator<IObservableStep<TContext, TReport>>? enumerator = null;
  public IObservableStep<TContext, TReport>? CurrentStep { get; protected set; } = default;
  public bool IsComplete { get; protected set; } = false;
  public ObservablePlan(IEnumerable<IObservableStep<TContext, TReport>> steps)
  {
    if (steps == null || !steps.Any())
      throw new ArgumentNullException("steps", "Steps couldn't be empty or equal `Null`");
    Steps = steps;
  }
  protected virtual void Init()
  {
    enumerator = Steps.GetEnumerator();
    CurrentStep = default;
    IsComplete = false;
  }
  public virtual void Restart()
  {
    Init();
  }
  public virtual IObservable<TReport> Execute(TContext context)
  {
    if (enumerator == null)
    {
      Init();

      if (enumerator == null)
        throw new ArgumentNullException("It's a miracle! Enumerator is null. Did you miss the initialization of enumerator?");
    }
    if (CurrentStep == null && !enumerator.MoveNext())
    {
      throw new ArgumentException("No steps to do");
    }
    CurrentStep = enumerator.Current;
    return CurrentStep.Make(context)
      .Expand(_report =>
      {
        if (IsSuccess(_report) && !IsComplete)
        {
          CurrentStep = enumerator.Current;
          return CurrentStep.Make(context);
        }
        return Observable.Empty<TReport>();
      })
      .Do(_report => { if (IsSuccess(_report)) IsComplete = !enumerator.MoveNext(); })
      .Do(_report => Console.WriteLine(CurrentStep.GetType().Name + ": " + _report));
  }
  protected abstract bool IsSuccess(TReport report);
}