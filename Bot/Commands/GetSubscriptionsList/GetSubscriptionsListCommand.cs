using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public class GetSubscriptionsListCommand(IMessageSender messageSender
  , IGetUserRelatedSirenas findSirena, IGetUserInformation getUserInformation
  , IFactory<IRequestContext, IEnumerable<SirenaData>, ISendMessageBuilder> messageBuilderFactory) 
  : AbstractBotCommmand(NAME, DESCRIPTION), IBotCommand//, IDisposable
{
  public const string NAME = "subscriptions";
  public const string DESCRIPTION = "Displays your current subscriptions.";
  CompositeDisposable disposables = new CompositeDisposable();

  public override void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;
    var info = context.GetCultureInfo();

    IDisposable userSubscriptionsStream = findSirena.GetSubscriptions(uid)
      .SelectMany(_sirenas => _sirenas)
      .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId,info)
          .Do(_nick => _sirena.OwnerNickname = _nick)
          .Select(_ => _sirena))
      .ToArray()
      .Select(_subscriptions => messageBuilderFactory.Create(context,_subscriptions))
      .SelectMany(messageSender.ObservableSend)
      .Subscribe();

    disposables.Add(userSubscriptionsStream);
  }

  public void Dispose()
  {
    disposables?.Dispose();
  }
}