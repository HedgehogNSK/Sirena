
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class GetSirenaInfoPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;

  public GetSirenaInfoPlanFactory(IFindSirenaOperation findSirenaOperation)
  {
    this.findSirenaOperation = findSirenaOperation;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> container = new();
    CommandStep[] steps = [
      new ValidateSirenaIdStep(contextContainer,container),
      new GetSirenaInfoStep(contextContainer,container,findSirenaOperation),
    ];
    return new(steps, contextContainer);
  }
}
