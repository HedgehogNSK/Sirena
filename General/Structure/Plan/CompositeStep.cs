using System.Reactive.Linq;

namespace Hedgey.Structure.Plan;
/// <summary>
/// Execute enumeration of steps. If one of internal steps is failed, 
/// then all step are failed. And on next execution all steps will be repeated again
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CompositeStep<T> : IObservableStep<T>
{
  public CompositeStep(IObservableStep<T>[] steps)
  {
    Steps = steps;
  }

  public IEnumerable<IObservableStep<T>> Steps { get; }

  public virtual IObservable<T> Make()
  {
    var enumerator = Steps.GetEnumerator();
    if(!enumerator.MoveNext())
      throw new ArgumentException("No more steps to do");
    return RecursiveMake(enumerator);
  }

  public IObservable<T> RecursiveMake(IEnumerator<IObservableStep<T>> enumerator)
  {
    var step = enumerator.Current;
    return step.Make()
      .SelectMany(x =>
      {
        if (!IsStepSuccesful(x) || !enumerator.MoveNext())
          return Observable.Return(x);
        return RecursiveMake(enumerator);
      });
  }

  protected abstract bool IsStepSuccesful(T x);
}