using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hedgey.Rx;

public class BottleNeck : IDisposable
{
  private IObservable<Unit> limitResetSampler;
  public IObservable<Unit> LimitResetSampler
  {
    get => limitResetSampler;
    set
    {
      limitResetSampler = value;
      Init();
    }
  }

  public int Limit { get; set; }
  public int passedCount;
  public int subscribers;
  private Subject<Unit> pass = new Subject<Unit>();
  private Subject<Unit> launchSampler = new Subject<Unit>();
  CompositeDisposable compositeDisposable = new CompositeDisposable();
  private bool inited;

  public BottleNeck(IObservable<Unit> resetLimitSampler, int limit, int count = 0)
  {
    this.passedCount = count;
    this.limitResetSampler = resetLimitSampler;
    this.Limit = limit;
  }
  public void Init()
  {
    compositeDisposable.Clear();

    var observableReset = launchSampler
      .SelectMany(limitResetSampler)
      .Publish();

    var resetStream = observableReset
      .Subscribe(Reset);
    compositeDisposable.Add(resetStream);

    var resetWaitersStream = observableReset
      .Where(_ => subscribers > 0)
      .SelectMany(_ => Observable.Repeat(Unit.Default, subscribers > Limit ? Limit : subscribers))
      .Subscribe(pass.OnNext);
    compositeDisposable.Add(resetWaitersStream);

    var resetStreamLauncher = observableReset.Connect();
    compositeDisposable.Add(resetStreamLauncher);

    inited = true;
  }

  public IObservable<Unit> Next()
  {
    if (!inited) Init();

    Interlocked.Increment(ref subscribers);

    return Observable.Return(Unit.Default).Merge(pass)
    .FirstAsync(_ => passedCount < Limit)
    .Do(ProcessEmission);
  }

  private void ProcessEmission(Unit unit)
  {
    Interlocked.Increment(ref passedCount);
    Interlocked.Decrement(ref subscribers);
    if (passedCount == 1)
      launchSampler.OnNext(unit);
  }
  public virtual void Reset(Unit _)
  {
    passedCount = 0;
  }
  public void Dispose()
  {
    compositeDisposable.Dispose();
    launchSampler.OnCompleted();
    pass.OnCompleted();
    launchSampler.Dispose();
    pass.Dispose();
  }
}