using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateRequestIsFromBot : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var message = context.GetMessage();
    if (!message.From.IsBot)
      return Observable.Return(new Report(Result.Canceled));
    return Observable.Return(new Report(Result.Success));
  }
}