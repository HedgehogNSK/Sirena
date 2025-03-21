using Hedgey.Sirena.Bot.Operations;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class GetUserCommandStep : CommandStep
{
  private readonly IUserInfoOperations getUserOperationAsync;
  private readonly CreateMessageBuilder messageBuilder;
  private readonly NullableContainer<UserStatistics> statsContainer;

  public GetUserCommandStep(IUserInfoOperations getUserOperationAsync
  , CreateMessageBuilder messageBuilder
  , NullableContainer<UserStatistics> statsContainer)
  : base()
  {
    this.getUserOperationAsync = getUserOperationAsync;
    this.messageBuilder = messageBuilder;
    this.statsContainer = statsContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    long uid = context.GetUser().Id;
    return getUserOperationAsync.Get(uid)
    .Select(CreateReport);
  }

  private Report CreateReport(UserStatistics userStats)
  {
    messageBuilder.SetUser(true);    
    statsContainer.Set(userStats);
    return new(Result.Success, null);
  }
}