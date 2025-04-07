using Hedgey.Extensions;
using Hedgey.Telegram.Bot;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateCallIdStep(NullableContainer<ObjectId> idContainer) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var param = context.GetArgsString().GetParameterByNumber(0);
    if (!ObjectId.TryParse(param, out var id))
      return Observable.Return(new Report(Result.Canceled));
    idContainer.Set(id);
    return Observable.Return(new Report(Result.Success));
  }
}