using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CommandPlan : ObservablePlan<CommandStep.Report>
{
  public readonly Container<IRequestContext> contextContainer;
  public CommandPlan(IEnumerable<IObservableStep< CommandStep.Report>> steps, Container<IRequestContext> context) : base(steps)
  {
    contextContainer = context;
  }
  public IRequestContext Context => contextContainer.Object;

  protected override bool IsStepSuccesful(CommandStep.Report report)
  {
    return report.Result == CommandStep.Result.Success;
  }
  public void Update(IRequestContext context) => contextContainer.Set(context);

  public record Report(CommandPlan Plan, CommandStep.Report StepReport);
}

