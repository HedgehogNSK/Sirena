using System.Reactive.Linq;

namespace Hedgey.Structure.Plan;
/// <summary>
/// Execute enumeration of steps. If one of internal steps is failed, 
/// then all step are failed. And on next execution all steps will be repeated again
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CompositeStep<T> : IObservableStep<T>
{
  public CompositeStep(params IObservableStep<T>[] steps)
  {
    if(steps.Length==0)
    {
       throw new ArgumentException("Input array has to be contains at least 1 step.", nameof(steps));
    }
    Steps = steps;
  }

  public IEnumerable<IObservableStep<T>> Steps { get; }

  public virtual IObservable<T> Make()
  {
    var enumerator = Steps.GetEnumerator();
    if (!enumerator.MoveNext())
      throw new ArgumentException("Steps collection is empty!");
    return enumerator.Current.Make()
      .Expand(_result =>
      {
        if (IsStepSuccesful(_result) && !enumerator.MoveNext())
        {
          return enumerator.Current.Make();
        }
        return Observable.Empty(_result);
      })
      .Do(_report => Console.WriteLine(enumerator.Current.GetType().Name + ": " + _report))
      .LastAsync();
  }

  protected abstract bool IsStepSuccesful(T x);
}