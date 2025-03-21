using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class DisplaySirenaRequestsStep(IGetUserInformation getUserInfromation
, NullableContainer<SirenRepresentation> sirenaContainer
, IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory
, IFactory<IRequestContext, SirenRepresentation, int, string, IEditMessageBuilder> editMessageBuilderFactory
, IFactory<IRequestContext,SirenRepresentation, ISendMessageBuilder> noRequestsMessageBuilderFactory)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    var chatID = context.GetTargetChatId();
    var sirena = sirenaContainer.Get();

    if (sirena.Requests.Length == 0)
    {
      var noRequestsForSirenaBuilder = noRequestsMessageBuilderFactory.Create(context, sirena);
      return Observable.Return(new Report(Result.Canceled, noRequestsForSirenaBuilder));
    }

    bool numberIsSet = false;
    var requestNumberString = context.GetArgsString().GetParameterByNumber(1);
    if (!int.TryParse(requestNumberString, out int requestID))
      requestID = 0;
    else
    {
      requestID = Math.Clamp(requestID, 0, sirena.Requests.Length - 1);
      numberIsSet = true;
    }

    var requestUID = sirena.Requests[requestID].UID;

    return getUserInfromation.GetNickname(requestUID, info)
      .Select(_userName =>
        {
          var result = sirena.Requests.Any() ? Result.Success : Result.Canceled;
          if (numberIsSet && context is CallbackRequestContext)
          {
            var editBuilder = editMessageBuilderFactory.Create(context, sirena, requestID, _userName);
            return new Report(result, EditMessageBuilder: editBuilder);
          }
          var builder = sendMessageBuilderFactory.Create(context, sirena, requestID, _userName);
          return new Report(result, builder);
        });
  }
  public class Factory(IGetUserInformation getUserInfromation
    , IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory
    , IFactory<IRequestContext, SirenRepresentation, int, string, IEditMessageBuilder> editMessageBuilderFactory
    , IFactory<IRequestContext,SirenRepresentation, ISendMessageBuilder> noRequestsMessageBuilderFactory)
 : IFactory<NullableContainer<SirenRepresentation>, DisplaySirenaRequestsStep>
  {
    public DisplaySirenaRequestsStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
     => new DisplaySirenaRequestsStep(getUserInfromation, sirenaContainer, sendMessageBuilderFactory, editMessageBuilderFactory,noRequestsMessageBuilderFactory);
  }
}