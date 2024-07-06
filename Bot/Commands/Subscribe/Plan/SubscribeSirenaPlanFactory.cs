using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class SubscribeSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly ISubscribeToSirenaOperation subscribeOperation;
  private readonly ILocalizationProvider localizationProvider;

  public SubscribeSirenaPlanFactory(ISubscribeToSirenaOperation subscribeOperation
  , ILocalizationProvider localizationProvider)
  {
    this.subscribeOperation = subscribeOperation;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> idContainer = new();
    IObservableStep< CommandStep.Report>[] steps = [
      new ValidateSirenaIdStep(contextContainer,idContainer, localizationProvider),
      new RequestSubscribeStep(contextContainer,idContainer, subscribeOperation, localizationProvider),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new([compositeStep], contextContainer);
  }
}
