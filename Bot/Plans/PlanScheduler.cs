using System.Reactive.Linq;
using System.Reactive.Subjects;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public class PlanScheduler : IDisposable
{
  readonly Subject<ExecutionSetings> subject = new Subject<ExecutionSetings>();
  public void Dispose()
  {
    subject.Dispose();
  }

  public void Push(CommandPlan plan, IRequestContext context)
  {
    var settings = new ExecutionSetings(plan, context);
    subject.OnNext(settings);
  }
  public IObservable<CommandPlan.Report> Track()
  {
    return subject.SelectMany(_settings => _settings.plan.Execute(_settings.context)
      .Select(_report => new CommandPlan.Report(_settings.plan, _settings.context, _report))
      );
  }
  private record ExecutionSetings(CommandPlan plan, IRequestContext context);
}