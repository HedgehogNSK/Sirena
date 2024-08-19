using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using RxTelegram.Bot;

namespace Hedgey.Sirena.Bot;

public class FindSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly TelegramBot bot;
  private readonly ILocalizationProvider localizationProvider;

  public FindSirenaPlanFactory(TelegramBot bot, IFindSirenaOperation findSirenaOperation
  , ILocalizationProvider localizationProvider)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.bot = bot;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      new ValidateSearchParamFindSirenaStep(localizationProvider),
      new RequestFindSirenaStep(findSirenaOperation, bot, localizationProvider)
    ];
    CompositeCommandStep compositeStep = new CompositeCommandStep(steps);

    return new(FindSirenaCommand.NAME, [compositeStep]);
  }
}