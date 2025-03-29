using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class CreateRequestInfoStep(NullableContainer<SirenRepresentation> sirenaContainer
, NullableContainer<RequestsCommand.RequestInfo> infoContainer)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var sirena = sirenaContainer.Get();
    var requestNumberString = context.GetArgsString().GetParameterByNumber(1);
    bool idIsSet = int.TryParse(requestNumberString, out int requestID);
    if (idIsSet)
      requestID = Math.Clamp(requestID, 0, sirena.Requests.Length - 1);

    infoContainer.Set(new(sirena, idIsSet, requestID));
    return Observable.Return(new Report(Result.Success));
  }
  public sealed class Factory : IFactory<NullableContainer<SirenRepresentation>
    , NullableContainer<RequestsCommand.RequestInfo>, CreateRequestInfoStep>
  {
    public CreateRequestInfoStep Create(NullableContainer<SirenRepresentation> sirena
    , NullableContainer<RequestsCommand.RequestInfo> requestInfo)
      => new CreateRequestInfoStep(sirena, requestInfo);
  }
}