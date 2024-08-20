using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class GetUserCommandStep : CommandStep
{
  private readonly IGetUserOperationAsync getUserOperationAsync;
  private readonly CreateMessageBuilder messageBuilder;
  private readonly NullableContainer<UserRepresentation> userContainer;

  public GetUserCommandStep(IGetUserOperationAsync getUserOperationAsync
  , CreateMessageBuilder messageBuilder
  , NullableContainer<UserRepresentation> userContainer)
  : base()
  {
    this.getUserOperationAsync = getUserOperationAsync;
    this.messageBuilder = messageBuilder;
    this.userContainer = userContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    return Observable.FromAsync(() => getUserOperationAsync.GetAsync(uid))
    .Select(CreateReport);
  }

  private Report CreateReport(UserRepresentation? representation)
  {
    if (representation == null)
    {
      messageBuilder.SetUser(representation);
      return new Report(Result.Wait, messageBuilder);
    }
    userContainer.Set(representation);
    return new(Result.Success, null);
  }
}