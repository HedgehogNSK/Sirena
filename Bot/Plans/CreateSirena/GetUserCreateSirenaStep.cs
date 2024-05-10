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
    return getUserOperationAsync.GetAsync(uid).ToObservable()
    .Select(CreateReport);
  }

  private Report CreateReport(UserRepresentation representation)
  {
    buffer.User = representation;
    buffer.MessageBuilder.SetUser(representation);
    return new Report(representation != null ? Result.Success : Result.CanBeFixed, buffer.MessageBuilder);
  }
}
