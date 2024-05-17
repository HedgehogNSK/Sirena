using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class SubscribeSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly ISubscribeToSirenaOperation subscribeOperation;

  public SubscribeSirenaPlanFactory(ISubscribeToSirenaOperation subscribeOperation)
  {
    this.subscribeOperation = subscribeOperation;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> idContainer = new();
    IEnumerable<IObservableStep< CommandStep.Report>> steps = [
      new ValidateSirenaIdStep(contextContainer,idContainer),
      new RequestSubscribeStep(contextContainer,idContainer, subscribeOperation),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new([compositeStep], contextContainer);
  }
}
