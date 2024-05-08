using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class CheckAbilityToCreateSirenaStep( Container<IRequestContext> contextContainer, CreateSirenaStep.Buffer buffer)
 : CreateSirenaStep(contextContainer,buffer)
{
  private const int SIGNAL_LIMIT = 5;

  public override IObservable<Report> Make()
  {
    var ownedSignalsCount = buffer.User.Owner.Length;    
    var result = ownedSignalsCount <SIGNAL_LIMIT? Result.Success: Result.Canceled;

    var builder = buffer.MessageBuilder;
    builder.IsUserAllowedToCreateSirena(result ==Result.Success);

    var report = new Report(result, builder);
    return Observable.Return(report);
  }
}