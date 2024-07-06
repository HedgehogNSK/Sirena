
using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class DisplaySirenaInfoPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly ILocalizationProvider localizationProvider;

  public DisplaySirenaInfoPlanFactory(IFindSirenaOperation findSirenaOperation, ILocalizationProvider localizationProvider)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> container = new();
    CommandStep[] steps = [
      new ValidateSirenaIdStep(contextContainer,container, localizationProvider),
      new GetSirenaInfoStep(contextContainer,container,findSirenaOperation, localizationProvider),
    ];
    return new(steps, contextContainer);
  }
}
