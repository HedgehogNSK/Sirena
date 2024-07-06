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

  public FindSirenaPlanFactory(IFindSirenaOperation findSirenaOperation
  , TelegramBot bot,
ILocalizationProvider localizationProvider)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.bot = bot;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    IObservableStep< CommandStep.Report>[] steps = [
      new ValidateSearchParamFindSirenaStep(contextContainer, localizationProvider),
      new RequestFindSirenaStep(contextContainer,findSirenaOperation,bot, localizationProvider),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new([compositeStep], contextContainer);
  }
}
