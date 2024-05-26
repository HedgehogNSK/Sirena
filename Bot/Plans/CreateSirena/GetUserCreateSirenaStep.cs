using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Hedgey.Sirena.Bot;

public class GetUserCreateSirenaStep : CreateSirenaStep
{
  private readonly IGetUserOperationAsync getUserOperationAsync;
  public GetUserCreateSirenaStep(Container<IRequestContext> contextContainer
    , Buffer buffer
    , IGetUserOperationAsync getUserOperationAsync)
  : base(contextContainer, buffer)
  {
    this.getUserOperationAsync = getUserOperationAsync;
  }

  public override IObservable<Report> Make()
  {
    User botUser = contextContainer.Object.GetUser();
    long uid = botUser.Id;
    return Observable.FromAsync(() =>getUserOperationAsync.GetAsync(uid))
    .Select(CreateReport);
  }

  private Report CreateReport(UserRepresentation representation)
  {
    buffer.User = representation;
    buffer.MessageBuilder.SetUser(representation);

    if (representation == null)
      return new Report(Result.Wait, buffer.MessageBuilder);

    return new(Result.Success, null);
  }
}