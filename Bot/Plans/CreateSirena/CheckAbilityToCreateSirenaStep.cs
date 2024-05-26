using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class CheckAbilityToCreateSirenaStep( Container<IRequestContext> contextContainer, CreateSirenaStep.Buffer buffer)
 : CreateSirenaStep(contextContainer,buffer)
{
  private const int SIGNAL_LIMIT = 5;

  public override IObservable<Report> Make()
  {
    var ownedSignalsCount = buffer.User.Owner.Length;
    var builder = buffer.MessageBuilder;

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