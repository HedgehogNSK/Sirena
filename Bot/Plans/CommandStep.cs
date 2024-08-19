using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public abstract class CommandStep : IObservableStep<IRequestContext, CommandStep.Report>
{
  public abstract IObservable<Report> Make(IRequestContext context);
  public enum Result
  {
    Success,
    /// <summary>
    /// Plan can't be executed with current params
    /// and it waits
    /// </summary>
    Wait,
    /// <summary>
    /// Command can't be executed with current params
    /// and it stops
    /// </summary>
    Canceled,
    /// <summary>
    /// Unhandled exception of the code behaviour
    /// Connection and request exceptions
    /// </summary>
    Exception,
  }

  public record class Report(Result Result, IMessageBuilder? MessageBuilder = null)
  {
    public override string ToString()
    {
      return $"Report: [State: {Result} | {MessageBuilder?.GetType().Name ?? "No message"}]";
    }
  }
}