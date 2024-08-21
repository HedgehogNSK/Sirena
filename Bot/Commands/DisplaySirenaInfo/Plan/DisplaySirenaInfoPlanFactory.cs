using Hedgey.Structure.Factory;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class DisplaySirenaInfoPlanFactory(
   IFactory<NullableContainer<ObjectId>, ValidateSirenaIdStep> idValidationStepFactory
    , IFactory<NullableContainer<ObjectId>, GetSirenaInfoStep> getSirenaInfoStepFactory)
    : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {

    NullableContainer<ObjectId> idContainer = new();
    CommandStep[] steps = [
     idValidationStepFactory.Create(idContainer),
    getSirenaInfoStepFactory.Create(idContainer)
    ];
    return new(DisplaySirenaInfoCommand.NAME, steps);
  }
}