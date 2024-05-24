using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using RxTelegram.Bot;

namespace Hedgey.Sirena.Bot;

public class FindSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly TelegramBot bot;

  public FindSirenaPlanFactory(IFindSirenaOperation findSirenaOperation
  , TelegramBot bot)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.bot = bot;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    IObservableStep< CommandStep.Report>[] steps = [
      new ValidateSearchParamFindSirenaStep(contextContainer),
      new RequestFindSirenaStep(contextContainer,findSirenaOperation,bot),
    ];
    var compositeStep = new CompositeCommandStep(steps);

    return new([compositeStep], contextContainer);
  }
}
