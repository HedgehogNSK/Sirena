using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class GetUserSirenasStep(IGetUserRelatedSirenas getUserSirenas
  , IGetUserInformation getUserInfo
  , IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory
  , IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory
  , SirenasListMessageBuilder userSirenasMessageBuilder)
   : CommandStep
{
  private readonly IGetUserRelatedSirenas getUserSirenas = getUserSirenas;
  private readonly IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory = noRequestsMessageFactory;
  private readonly IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory = sendMessageBuilderFactory;
  private readonly SirenasListMessageBuilder userSirenasMessageBuilder = userSirenasMessageBuilder;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var observableSirenas = getUserSirenas.GetSirenasWithRequests(uid).Publish().RefCount(2);

    var observableSingleSirena = observableSirenas
      .Where(_sirenas => _sirenas.Any() && !_sirenas.Skip(1).Any())
      .SelectMany(x => CreateMessageForSingle(x.Single()));

    var observableEmptyAndManySirenas = observableSirenas
      .Where(_sirenas => !_sirenas.Any() || _sirenas.Skip(1).Any())
      .Select(CreateReport);

    return observableEmptyAndManySirenas.Merge(observableSingleSirena);

    IObservable<Report> CreateMessageForSingle(SirenRepresentation sirena)
    {
      var requestIdString = context.GetArgsString().GetParameterByNumber(1);
      var requestInfo = RequestsCommand.Create(sirena, requestIdString);

      return getUserInfo.GetNickname(requestInfo.RequestorID)
        .Select(_username =>
          new Report(Result.Canceled
            , sendMessageBuilderFactory.Create(context, sirena, requestInfo.RequestID, _username)
          )
        );
    }
    Report CreateReport(IEnumerable<SirenRepresentation> sirenas)
    {
      if (!sirenas.Any())
      {
        ISendMessageBuilder builder = noRequestsMessageFactory.Create(context);
        return new Report(Result.Canceled, builder);
      }

      userSirenasMessageBuilder.SetSirenas(sirenas);
      return new Report(Result.Success, null);
    }
  }

  public class Factory(IGetUserRelatedSirenas getUserSirenas
  , IGetUserInformation getUserInfo
  , IFactory<IRequestContext, SirenRepresentation, int, string, ISendMessageBuilder> sendMessageBuilderFactory
    , IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory)
     : IFactory<SirenasListMessageBuilder, GetUserSirenasStep>
  {
    public GetUserSirenasStep Create(SirenasListMessageBuilder userSirenasMessageBuilder
      )
        => new GetUserSirenasStep(getUserSirenas, getUserInfo
          , noRequestsMessageFactory, sendMessageBuilderFactory
          , userSirenasMessageBuilder);
  }
}