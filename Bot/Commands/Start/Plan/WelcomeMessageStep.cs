using Hedgey.Blendflake;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class WelcomeMessageStep(
    IFactory<IRequestContext, UserStatistics, ISendMessageBuilder> messageBuilder
    , IUserInfoOperations userInfo
    , NullableContainer<UpdateState> updateStateContainer)
 : CommandStep
{
  private readonly IFactory<IRequestContext, UserStatistics, ISendMessageBuilder> messageBuilder = messageBuilder;
  private readonly IUserInfoOperations userInfo = userInfo;
  private readonly NullableContainer<UpdateState> updateStateContainer = updateStateContainer;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    return userInfo.Get(uid)
      .Select(userStats =>
      {
        var arg = context.GetArgsString();
        if (HashUtilities.TryParse(arg, out ulong id) && UserExists(userStats))
          return new Report(Result.Success);
        return new Report(Result.Success, messageBuilder.Create(context, userStats));
      });
  }
  private bool UserExists(UserStatistics userStats)
  {
    return (updateStateContainer.Content & UpdateState.Exists) != 0;
  }

  public class Factory(IFactory<IRequestContext, UserStatistics, ISendMessageBuilder> messageBuilder
    , IUserInfoOperations userInfo) 
    : IFactory<NullableContainer<UpdateState>, WelcomeMessageStep>
  {
    private readonly IFactory<IRequestContext, UserStatistics, ISendMessageBuilder> messageBuilder = messageBuilder;
    private readonly IUserInfoOperations userInfo = userInfo;

    public WelcomeMessageStep Create(NullableContainer<UpdateState> updateState) 
      => new WelcomeMessageStep(messageBuilder, userInfo, updateState);
  }
}