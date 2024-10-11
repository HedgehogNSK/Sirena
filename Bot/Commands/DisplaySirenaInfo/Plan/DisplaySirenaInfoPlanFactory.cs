using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class DisplaySirenaInfoPlanFactory(
   IFactory<NullableContainer<ulong>, ValidateSirenaIdStep> idValidationStepFactory
    , IFactory<NullableContainer<ulong>, GetSirenaInfoStep> getSirenaInfoStepFactory)
    : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {

    NullableContainer<ulong> idContainer = new();
    CommandStep[] steps = [
     idValidationStepFactory.Create(idContainer),
    getSirenaInfoStepFactory.Create(idContainer)
    ];
    return new(DisplaySirenaInfoCommand.NAME, steps);
  }
}