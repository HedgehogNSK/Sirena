using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class GetSubscriptionsListCommand : AbstractBotCommmand, IBotCommand, IDisposable
{
  public const string NAME = "subscriptions";
  public const string DESCRIPTION = "Displays you current subscriptions.";
  CompositeDisposable disposables = new CompositeDisposable();
  private readonly IMessageSender messageSender;
  private readonly IGetUserRelatedSirenas findSirena;
  private readonly IGetUserInformation getUserInformation;

  public GetSubscriptionsListCommand(IMessageSender messageSender
  , IGetUserRelatedSirenas findSirena, IGetUserInformation getUserInformation)
    : base(NAME, DESCRIPTION)
  {
    this.messageSender = messageSender;
    this.findSirena = findSirena;
    this.getUserInformation = getUserInformation;
  }

  public override void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;

    IDisposable userSubscriptionsStream = findSirena.GetSubscriptions(uid)
      .SelectMany(_sirenas => _sirenas)
      .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId)
          .Select(_nick => (_sirena, _nick)))
      .ToArray().Subscribe(_subscriptions => ProcessResult(_subscriptions, chatId));

    disposables.Add(userSubscriptionsStream);
  }

  private void ProcessResult((SirenRepresentation _sirena, string _nick)[] subscriptions, long chatId)
  {
    MessageBuilder builder = new SubscriptionsMesssageBuilder(chatId, subscriptions);
    messageSender.Send(builder.Build());
  }

  public void Dispose()
  {
    disposables?.Dispose();
  }
}