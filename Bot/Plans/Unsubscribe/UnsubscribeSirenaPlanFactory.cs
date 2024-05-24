using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IGetUserRelatedSirenas getSubscriptions;
  private readonly IGetUserInformation getUserInformation;
  private readonly IUnsubscribeSirenaOperation unsubscribeSirenaOperation;
  public UnsubscribeSirenaPlanFactory( IGetUserInformation getUserInformation
  , IGetUserRelatedSirenas getSubscriptions, IUnsubscribeSirenaOperation unsubscribeSirenaOperation)
  {
    this.getUserInformation = getUserInformation;
    this.getSubscriptions = getSubscriptions;
    this.unsubscribeSirenaOperation = unsubscribeSirenaOperation;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> idContainer = new();
    IObservableStep< CommandStep.Report>[] steps = [
      new ProcessParameterUnsubscribeStep(contextContainer,idContainer,getSubscriptions,getUserInformation),
      new TryUnsubscribeStep(contextContainer,idContainer, unsubscribeSirenaOperation),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new([compositeStep], contextContainer);
  }
}
