using System.Reactive.Linq;
using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;

public class CheckAbilityToCreateSirenaStep : CommandStep
{
  private const int SIGNAL_LIMIT = 5;
  private readonly NullableContainer<UserRepresentation> userContainer;
  private readonly Container<CreateMessageBuilder> messageBuilderContainer;

  public CheckAbilityToCreateSirenaStep(NullableContainer<UserRepresentation> userContainer, Container<CreateMessageBuilder> messageBuilderContainer) : base()
  {
    this.userContainer = userContainer;
    this.messageBuilderContainer = messageBuilderContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var ownedSignalsCount = userContainer.Get().Owner.Length;
    var builder = messageBuilderContainer.Object;

    Report report;
    if (ownedSignalsCount < SIGNAL_LIMIT)
    {
      builder.IsUserAllowedToCreateSirena(true);
      report = new Report(Result.Success, null);
    }
    else
    {
      builder.IsUserAllowedToCreateSirena(false);
      report = new Report(Result.Canceled, builder);
    }
    return Observable.Return(report);
  }
}