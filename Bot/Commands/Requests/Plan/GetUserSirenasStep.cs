using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class GetUserSirenasStep(IGetUserInformation getUserInfo
  , NullableContainer<IEnumerable<SirenRepresentation>> sirenaContainer
  , IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory
  , IFactory<IRequestContext, RequestsCommand.RequestInfo, ISendMessageBuilder> sendMessageBuilderFactory
  , SirenasListMessageBuilder userSirenasMessageBuilder)
   : CommandStep
{
  private readonly NullableContainer<IEnumerable<SirenRepresentation>> sirenaContainer = sirenaContainer;
  private readonly IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory = noRequestsMessageFactory;
  private readonly IFactory<IRequestContext, RequestsCommand.RequestInfo, ISendMessageBuilder> sendMessageBuilderFactory = sendMessageBuilderFactory;
  private readonly SirenasListMessageBuilder userSirenasMessageBuilder = userSirenasMessageBuilder;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var _sirenas = sirenaContainer.Get();

    if (_sirenas.Any() && !_sirenas.Skip(1).Any())
      return CreateMessageForSingle(context, _sirenas.Single());

    var report = CreateReport(context, _sirenas);
    return Observable.Return(report);

  }
  IObservable<Report> CreateMessageForSingle(IRequestContext context, SirenRepresentation sirena)
  {
    var requestIdString = context.GetArgsString().GetParameterByNumber(1);
    var requestInfo = RequestsCommand.Create(sirena, requestIdString);

    return getUserInfo.GetNickname(requestInfo.RequestorID)
      .Select(_username =>
      {
        requestInfo.Username = _username;
        return new Report(Result.Success, sendMessageBuilderFactory.Create(context, requestInfo));
      });
  }
  Report CreateReport(IRequestContext context, IEnumerable<SirenRepresentation> sirenas)
  {
    if (!sirenas.Any())
    {
      ISendMessageBuilder builder = noRequestsMessageFactory.Create(context);
      return new Report(Result.Wait, builder);
    }

    return new Report(Result.Success, userSirenasMessageBuilder.SetSirenas(sirenas));
  }

  public class Factory(IGetUserInformation getUserInfo
  , IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory
  , IFactory<IRequestContext, RequestsCommand.RequestInfo, ISendMessageBuilder> sendMessageBuilderFactory)
     : IFactory<SirenasListMessageBuilder, NullableContainer<IEnumerable<SirenRepresentation>>, GetUserSirenasStep>
  {
    public GetUserSirenasStep Create(SirenasListMessageBuilder userSirenasMessageBuilder
    , NullableContainer<IEnumerable<SirenRepresentation>> sirenaContainer)
        => new GetUserSirenasStep(getUserInfo, sirenaContainer
          , noRequestsMessageFactory, sendMessageBuilderFactory
          , userSirenasMessageBuilder);
  }
}