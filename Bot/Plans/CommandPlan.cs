using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CommandPlan : ObservablePlan<IRequestContext, CommandStep.Report>
{
  public readonly string commandName;
  public CommandPlan(string commandName, IEnumerable<IObservableStep<IRequestContext, CommandStep.Report>> steps)
   : base(steps)
  {
    this.commandName = commandName;
  }

  protected override bool IsSuccess(CommandStep.Report report)
  {
    return report.Result == CommandStep.Result.Success;
  }
  public record Report(CommandPlan Plan, IRequestContext Context, CommandStep.Report StepReport);
}