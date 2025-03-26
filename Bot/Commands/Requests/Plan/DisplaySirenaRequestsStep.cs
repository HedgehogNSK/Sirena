using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class DisplaySirenaRequestsStep(IGetUserInformation getUserInfromation
, NullableContainer<RequestsCommand.RequestInfo> infoContainer
, IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory
, IFactory<IRequestContext, SirenRepresentation, int, string, IEditMessageBuilder> editMessageBuilderFactory
, IFactory<IRequestContext, SirenRepresentation, ISendMessageBuilder> noRequestsMessageBuilderFactory)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    var requestInfo = infoContainer.Get();
    var sirena = requestInfo.Sirena;

    if (sirena.Requests.Length == 0)
    {
      var noRequestsForSirenaBuilder = noRequestsMessageBuilderFactory.Create(context, sirena);
      return Observable.Return(new Report(Result.Canceled, noRequestsForSirenaBuilder));
    }

    return getUserInfromation.GetNickname(requestInfo.RequestorID, info)
      .Select(GetReport);

    Report GetReport(string username)
    {
      if (requestInfo.isExplicitID && context is CallbackRequestContext)
      {
        var editBuilder = editMessageBuilderFactory.Create(context, sirena, requestInfo.RequestID, username);
        return new Report(Result.Success, EditMessageBuilder: editBuilder);
      }
      var builder = sendMessageBuilderFactory.Create(context, sirena, requestInfo.RequestID, username);
      return new Report(Result.Success, builder);
    }
  }
  public class Factory(IGetUserInformation getUserInfromation
    , IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory
    , IFactory<IRequestContext, SirenRepresentation, int, string, IEditMessageBuilder> editMessageBuilderFactory
    , IFactory<IRequestContext, SirenRepresentation, ISendMessageBuilder> noRequestsMessageBuilderFactory)
 : IFactory<NullableContainer<RequestsCommand.RequestInfo>, DisplaySirenaRequestsStep>
  {
    public DisplaySirenaRequestsStep Create(NullableContainer<RequestsCommand.RequestInfo> infoContainer)
     => new DisplaySirenaRequestsStep(getUserInfromation, infoContainer
      , sendMessageBuilderFactory, editMessageBuilderFactory
      , noRequestsMessageBuilderFactory);
  }
}