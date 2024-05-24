using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CompositeCommandStep : CompositeStep<CommandStep.Report>
{
  public CompositeCommandStep(params IObservableStep<CommandStep.Report>[] steps) 
  : base(steps)
  {
  }

  protected override bool IsStepSuccesful(CommandStep.Report report)
  {
    return report.Result == CommandStep.Result.Success;
  }
}