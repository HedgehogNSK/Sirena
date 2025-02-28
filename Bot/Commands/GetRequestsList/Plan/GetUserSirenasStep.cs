using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class GetUserSirenasStep(IGetUserRelatedSirenas getUserSirenas
  , IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory
  , SirenasListMessageBuilder userSirenasMessageBuilder)
   : CommandStep
{
  private readonly IGetUserRelatedSirenas getUserSirenas = getUserSirenas;
  private readonly IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory = noRequestsMessageFactory;
  private readonly SirenasListMessageBuilder userSirenasMessageBuilder = userSirenasMessageBuilder;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var searchKey = context.GetArgsString();
    var uid = context.GetUser().Id;

    return getUserSirenas.GetSirenasWithRequests(uid).Select(CreateReport);

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
    , IFactory<IRequestContext, ISendMessageBuilder> noRequestsMessageFactory)
     : IFactory<SirenasListMessageBuilder, GetUserSirenasStep>
  {
    public GetUserSirenasStep Create(SirenasListMessageBuilder userSirenasMessageBuilder)
      => new GetUserSirenasStep(getUserSirenas, noRequestsMessageFactory, userSirenasMessageBuilder);
  }
}