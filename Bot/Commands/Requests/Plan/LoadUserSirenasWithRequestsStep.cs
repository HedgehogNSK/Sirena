using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class LoadUserSirenasWithRequestsStep(IGetUserRelatedSirenas sirenasLoader
  , NullableContainer<IEnumerable<SirenaData>> sirenasContainer)
   : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    return sirenasLoader.GetSirenasWithRequests(uid).Select(x =>
    {
      sirenasContainer.Set(x);
      return new Report(Result.Success);
    });
  }
  public class Factory(IGetUserRelatedSirenas sirenasLoader)
   : IFactory<NullableContainer<IEnumerable<SirenaData>>, LoadUserSirenasWithRequestsStep>
  {
    public LoadUserSirenasWithRequestsStep Create(NullableContainer<IEnumerable<SirenaData>> container)
    => new LoadUserSirenasWithRequestsStep(sirenasLoader, container);
  }
}
