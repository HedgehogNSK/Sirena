using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class RequestSubscribeStep : CommandStep
{
  private readonly NullableContainer<ObjectId> sirenaIdContainter;
  private readonly ISubscribeToSirenaOperation subscribeOperation;

  public RequestSubscribeStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> sirenaIdContainter, ISubscribeToSirenaOperation subscribeOperation)
  : base(contextContainer)
  {
    this.sirenaIdContainter = sirenaIdContainter;
    this.subscribeOperation = subscribeOperation;
  }

  public override IObservable<Report> Make()
  {
    var id = sirenaIdContainter.Object;
    var uid = contextContainer.Object.GetUser().Id;

    var request = subscribeOperation.Subscribe(uid, id).Publish().RefCount();
    var fail = request.Where(x => x == null).Select(_ => CreateReportNotFound());
    var success = request.Where(x => x != null).Select(CreateSuccesfulReport);
    return success.Merge(fail);
  }

  private Report CreateSuccesfulReport(SirenRepresentation representation)
  {
    var chatId = contextContainer.Object.GetTargetChatId();
    MessageBuilder meesage = new SuccesfulSubscriptionMessageBuilder(chatId, representation);
    return new Report(Result.Success, meesage);
  }

  private Report CreateReportNotFound()
  {
    var id = sirenaIdContainter.Object;
    var chatId = contextContainer.Object.GetTargetChatId();
    return new(Result.Wait, new SirenaNotFoundMessageBuilder(chatId, id));
  }
}
