using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class GetUserSirenaStep(NullableContainer<ulong> idContainer
  , NullableContainer<SirenRepresentation> sirenaContainer
  , IGetUserRelatedSirenas findSirena
  , ISendMessageBuilder sirenaNotFoundMessageBuilder)
  : CommandStep
{

  public override IObservable<Report> Make(IRequestContext context)
  {
    var sid = idContainer.Get();
    long uid = context.GetUser().Id;

    return findSirena.GetUserSirenaOrNull(uid, sid).Select(Process);

    Report Process(SirenRepresentation source)
    {
      if (source == null)
        return new Report(Result.Canceled, sirenaNotFoundMessageBuilder);

      sirenaContainer.Set(source);
      return new Report(Result.Success);
    }
  }

  public class Factory(IGetUserRelatedSirenas getUserSirena)
     : IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, ISendMessageBuilder, GetUserSirenaStep>
  {
    public GetUserSirenaStep Create(NullableContainer<ulong> idContainer
      , NullableContainer<SirenRepresentation> sirenaContainer
      , ISendMessageBuilder sirenaNotFoundMessageBuilder)
      => new GetUserSirenaStep(idContainer, sirenaContainer, getUserSirena, sirenaNotFoundMessageBuilder);
  }
}