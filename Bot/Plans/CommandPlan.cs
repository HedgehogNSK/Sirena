using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CommandPlan : ObservablePlan<CommandStep, CommandStep.Report, CommandPlan.Report>
{
  public readonly Container<IRequestContext> contextContainer;
  public CommandPlan(IEnumerable<CommandStep> steps, Container<IRequestContext> context) : base(steps)
  {
    contextContainer = context;
  }

  protected override Report CreateSummary(CommandStep.Report report)
  => new Report(this, CurrentStep, report);

  protected override bool IsStepSuccesful(CommandStep.Report report)
  {
    return report.Result == CommandStep.Result.Success;
  }

  public enum Result
  {
    Success,
    /// <summary>
    /// Plan can't be executed with current params
    /// and it waits
    /// </summary>
    CanBeFixed,
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
  public record class Report(CommandPlan Plan, CommandStep? LastStep, CommandStep.Report LastStepReport);
}

