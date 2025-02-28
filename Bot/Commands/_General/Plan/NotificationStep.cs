using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public abstract class NotificationStep : CommandStep
{
  private bool userNotified = false;
  public NotificationStep()
  { }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var message = context.GetMessage();
    Report report;
    if ((string.IsNullOrEmpty(context.GetCommandName()) || message.From.IsBot) && !userNotified)
    {
      userNotified = true;
      report = new(Result.Wait, CreateNotification(context));
    }
    else
    {
      report = new(Result.Success);
    }
    return Observable.Return(report);
  }

  protected abstract ISendMessageBuilder CreateNotification(IRequestContext context);
}