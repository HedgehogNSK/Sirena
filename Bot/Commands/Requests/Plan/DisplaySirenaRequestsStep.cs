using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class DisplaySirenaRequestsStep(IGetUserInformation getUserInfromation
, NullableContainer<SirenRepresentation> sirenaContainer
, IFactory<IRequestContext, RequestsCommand.RequestInfo, ISendMessageBuilder> sendMessageBuilderFactory
, IFactory<IRequestContext, RequestsCommand.RequestInfo, IEditMessageBuilder> editMessageBuilderFactory
, IFactory<IRequestContext, SirenRepresentation, ISendMessageBuilder> noRequestsMessageBuilderFactory)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    var sirena = sirenaContainer.Get();
    var requestIdString = context.GetArgsString().GetParameterByNumber(1);
    var requestInfo = RequestsCommand.Create(sirena, requestIdString);
    if (sirena.Requests.Length == 0)
    {
      var noRequestsForSirenaBuilder = noRequestsMessageBuilderFactory.Create(context, sirena);
      var fallback = new FallbackRequestContext(context, RequestsCommand.NAME);
      return Observable.Return(new Report(fallback , noRequestsForSirenaBuilder));
    }

    return getUserInfromation.GetNickname(requestInfo.RequestorID, info)
      .Select(GetReport);

    Report GetReport(string username)
    {
      requestInfo.Username = username;
      if (requestInfo.isExplicitID && context is CallbackRequestContext)
      {
        var editBuilder = editMessageBuilderFactory.Create(context, requestInfo);
        return new Report(Result.Success, EditMessageBuilder: editBuilder);
      }
      var builder = sendMessageBuilderFactory.Create(context, requestInfo);
      return new Report(Result.Success, builder);
    }
  }
  public class Factory(IGetUserInformation getUserInfromation
, IFactory<IRequestContext, RequestsCommand.RequestInfo, ISendMessageBuilder> sendMessageBuilderFactory
, IFactory<IRequestContext, RequestsCommand.RequestInfo, IEditMessageBuilder> editMessageBuilderFactory
    , IFactory<IRequestContext, SirenRepresentation, ISendMessageBuilder> noRequestsMessageBuilderFactory)
 : IFactory<NullableContainer<SirenRepresentation>, DisplaySirenaRequestsStep>
  {
    public DisplaySirenaRequestsStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
     => new DisplaySirenaRequestsStep(getUserInfromation, sirenaContainer
      , sendMessageBuilderFactory, editMessageBuilderFactory
      , noRequestsMessageBuilderFactory);
  }
}