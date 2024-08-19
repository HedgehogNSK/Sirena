using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CompositeCommandStep : CompositeStep<IRequestContext, CommandStep.Report>
{
  public CompositeCommandStep(params IObservableStep<IRequestContext, CommandStep.Report>[] steps)
  : base(steps)
  { }

  protected override bool IsStepSuccesful(CommandStep.Report report)
  {
    return report.Result == CommandStep.Result.Success;
  }
}