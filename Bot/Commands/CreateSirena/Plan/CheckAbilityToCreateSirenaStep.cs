using Hedgey.Sirena.Bot.Operations;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class CheckAbilityToCreateSirenaStep : CommandStep
{
  private const int SIGNAL_LIMIT = 5;
  private readonly NullableContainer<UserStatistics> statsContainer;
  private readonly CreateMessageBuilder messageBuilder;

  public CheckAbilityToCreateSirenaStep(NullableContainer<UserStatistics> statsContainer, CreateMessageBuilder messageBuilder) : base()
  {
    this.statsContainer = statsContainer;
    this.messageBuilder = messageBuilder;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var ownedSignalsCount = statsContainer.Get().SirenasCount;

    Report report;
    if (ownedSignalsCount < SIGNAL_LIMIT)
    {
      messageBuilder.IsUserAllowedToCreateSirena(true);
      report = new Report(Result.Success, null);
    }
    else
    {
      messageBuilder.IsUserAllowedToCreateSirena(false);
      report = new Report(Result.Canceled, messageBuilder);
    }
    return Observable.Return(report);
  }
}