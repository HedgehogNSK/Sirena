using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Hedgey.Sirena.Bot;
public class PlanScheduler : IDisposable{
  readonly Subject<CommandPlan> subject= new Subject<CommandPlan>();
  public void Dispose()
  {
    subject.OnCompleted();
    subject.Dispose();
  }

  public void Push(CommandPlan plan){
      subject.OnNext(plan);
  }
  public IObservable<CommandPlan.Report> Track(){
     return subject.SelectMany(x=> x.Execute());
  }
}