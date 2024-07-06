using Hedgey.Localization;
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
  private readonly ILocalizationProvider localizationProvider;

  public UnsubscribeSirenaPlanFactory(IGetUserInformation getUserInformation
  , IGetUserRelatedSirenas getSubscriptions, IUnsubscribeSirenaOperation unsubscribeSirenaOperation
  , ILocalizationProvider localizationProvider)
  {
    this.getUserInformation = getUserInformation;
    this.getSubscriptions = getSubscriptions;
    this.unsubscribeSirenaOperation = unsubscribeSirenaOperation;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> idContainer = new();
    IObservableStep<CommandStep.Report>[] steps = [
      new ProcessParameterUnsubscribeStep(contextContainer,idContainer,getSubscriptions,getUserInformation, localizationProvider),
      new TryUnsubscribeStep(contextContainer,idContainer, unsubscribeSirenaOperation, localizationProvider),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new([compositeStep], contextContainer);
  }
}
