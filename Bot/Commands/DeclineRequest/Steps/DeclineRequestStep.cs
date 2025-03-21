using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public sealed class DeclineRequestStep(IRightsManageOperation rightsManager, IGetUserInformation getUser
, DeclineRequestMessageBuilder declineMessageBuilder
, NullableContainer<SirenRepresentation> sirenContainer
  ) : CommandStep
{
  private readonly IRightsManageOperation rightsManager = rightsManager;
  private readonly IGetUserInformation getUser = getUser;
  private readonly NullableContainer<SirenRepresentation> sirenContainer = sirenContainer;

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
      declineMessageBuilder.NoRequestor(context, sirena, key);
      return Observable.Return(new Report(Result.Canceled, declineMessageBuilder));
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

  public class Factory(IRightsManageOperation rightsManager, IGetUserInformation getUser, DeclineRequestMessageBuilder declineMessageBuilder)
    : IFactory<NullableContainer<SirenRepresentation>, DeclineRequestStep>
  {
    public DeclineRequestStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
      => new DeclineRequestStep(rightsManager, getUser, declineMessageBuilder, sirenaContainer);
  }
}