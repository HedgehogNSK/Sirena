using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public sealed class DeclineRequestStep(IRightsManageOperation rightsManager, IGetUserInformation getUser
  , DeclineRequestMessageBuilder declineMessageBuilder
  , NullableContainer<SirenaData> sirenContainer
) : CommandStep
{
  private readonly IRightsManageOperation rightsManager = rightsManager;
  private readonly IGetUserInformation getUser = getUser;
  private readonly NullableContainer<SirenaData> sirenContainer = sirenContainer;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var sirena = sirenContainer.Get();
    if (sirena.OwnerId != context.GetUser().Id)
    {
      declineMessageBuilder.NotAllowed(context, sirena);
      return Observable.Return(new Report(Result.Canceled, declineMessageBuilder));
    }

    if (sirena.Requests.Length == 0)
    {
      declineMessageBuilder.NoRequests(context, sirena);
      return Observable.Return(new Report(Result.Canceled, declineMessageBuilder));
    }

    var key = context.GetArgsString().GetParameterByNumber(1);
    if (!long.TryParse(key, out var requestorId))
    {
      var fallback = new FallbackRequestContext(context, RequestsCommand.NAME, sirena.ShortHash);
      return Observable.Return(new Report(fallback));
    }

    var getUsername = getUser.GetNickname(requestorId);
    if (sirena.Requests.All(x => x.UID != requestorId))
    {
      return getUsername.Select(SubstituteUsername);
    }

    return rightsManager.Decline(sirena.SID, requestorId)
      .CombineLatest(getUsername)
      .Select((_result) =>
      {
        if (!_result.First)
          //TODO: Change to Report(Result.Exception..)
          throw new InvalidOperationException($"Attempt to decline Sirena request failed. Parameters: (SID: {sirena.SID}, UID: {requestorId})");
        declineMessageBuilder.Success(context, sirena, _result.Second);
        return new Report(Result.Success, declineMessageBuilder);
      });

    Report SubstituteUsername(string userName)
    {
      declineMessageBuilder.NoRequestor(context, sirena, userName);
      return new Report(Result.Canceled, declineMessageBuilder);
    }
  }

  public class Factory(IRightsManageOperation rightsManager, IGetUserInformation getUser
    , DeclineRequestMessageBuilder.Factory declineMessageBuilderFactory)
    : IFactory<NullableContainer<SirenaData>, DeclineRequestStep>
  {
    public DeclineRequestStep Create(NullableContainer<SirenaData> sirenaContainer)
    {
      var declineMessageBuilder = declineMessageBuilderFactory.Create();
      return new DeclineRequestStep(rightsManager, getUser, declineMessageBuilder, sirenaContainer);
    }
  }
}