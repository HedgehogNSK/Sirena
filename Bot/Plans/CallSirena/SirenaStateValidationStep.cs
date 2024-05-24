using Hedgey.Sirena.Database;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaStateValidationStep : CommandStep
{
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  static public readonly TimeSpan allowedCallPeriod = TimeSpan.FromMinutes(1);

  public SirenaStateValidationStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer)
  : base(contextContainer)
  {
    this.sirenaContainer = sirenaContainer;
  }

  public override IObservable<Report> Make()
  {
    long uid = Context.GetUser().Id;
    long chatId = Context.GetTargetChatId();

    SirenRepresentation sirena = sirenaContainer.Get();
    Report report;
    if (sirena.CanBeCalledBy(uid) && IsReadyToCall(sirena))
    {
      report = new Report(Result.Success,null);
    }
    else
      report = new Report(Result.Canceled, new NotAllowedToCallMessageBuilder(chatId, sirena, uid));
    return Observable.Return(report);
  }

  private static bool IsReadyToCall(SirenRepresentation sirena)
  {
    if (sirena.LastCall == null)
      return true;
    
    var timePassed = DateTimeOffset.UtcNow - sirena.LastCall.Date;
    return timePassed > allowedCallPeriod;
  }
}
