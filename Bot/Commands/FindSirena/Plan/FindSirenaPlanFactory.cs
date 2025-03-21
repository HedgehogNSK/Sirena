using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class FindSirenaPlanFactory(IGetUserInformation getUserInformation
  , IFindSirenaOperation findSirenaOperation
  , ILocalizationProvider localizationProvider) 
  : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation = findSirenaOperation;
  private readonly IGetUserInformation getUserInformation = getUserInformation;
  private readonly ILocalizationProvider localizationProvider = localizationProvider;

  public CommandPlan Create(IRequestContext context)
  {
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      new ValidateSearchParamFindSirenaStep(localizationProvider),
      new RequestFindSirenaStep(findSirenaOperation, getUserInformation, localizationProvider)
    ];
    CompositeCommandStep compositeStep = new CompositeCommandStep(steps);

    return new(FindSirenaCommand.NAME, [compositeStep]);
  }
}