using Hedgey.Sirena.Database;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class CheckAbilityToCreateSirenaStep : CommandStep
{
  private const int SIGNAL_LIMIT = 5;
  private readonly NullableContainer<UserRepresentation> userContainer;
  private readonly CreateMessageBuilder messageBuilder;

  public CheckAbilityToCreateSirenaStep(NullableContainer<UserRepresentation> userContainer, CreateMessageBuilder messageBuilder) : base()
  {
    this.userContainer = userContainer;
    this.messageBuilder = messageBuilder;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var ownedSignalsCount = userContainer.Get().Owner.Length;

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