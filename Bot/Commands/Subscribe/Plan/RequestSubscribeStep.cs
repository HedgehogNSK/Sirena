using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class RequestSubscribeStep(NullableContainer<ObjectId> sirenaIdContainter
  , ISubscribeToSirenaOperation subscribeOperation
  , IFactory<IRequestContext, SirenRepresentation, SuccesfulSubscriptionMessageBuilder> successMessagBuilderFactory
  , IFactory<IRequestContext, ObjectId, SirenaNotFoundMessageBuilder> sirenaNotFoundMessageBuilderFactory)
  : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var id = sirenaIdContainter.Get();
    var uid = context.GetUser().Id;

    var request = subscribeOperation.Subscribe(uid, id).Publish().RefCount();
    var fail = request.Where(x => x == null).Select(_ => CreateReportNotFound());
    var success = request.Where(x => x != null).Select(CreateSuccesfulReport);
    return success.Merge(fail);

    Report CreateSuccesfulReport(SirenRepresentation representation)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      MessageBuilder meesage = successMessagBuilderFactory.Create(context, representation);
      return new Report(Result.Success, meesage);
    }
    Report CreateReportNotFound()
    {
      var id = sirenaIdContainter.Get();
      var info = context.GetCultureInfo();
      var chatId = context.GetTargetChatId();
      return new(Result.Wait, sirenaNotFoundMessageBuilderFactory.Create(context, id));
    }
  }
  public class Factory(ISubscribeToSirenaOperation subscribeOperation
  , IFactory<IRequestContext, SirenRepresentation, SuccesfulSubscriptionMessageBuilder> successMessagBuilderFactory
  , IFactory<IRequestContext, ObjectId, SirenaNotFoundMessageBuilder> sirenaNotFoundMessageBuilderFactory)
  : IFactory<NullableContainer<ObjectId>, RequestSubscribeStep>
  {
    public RequestSubscribeStep Create(NullableContainer<ObjectId> idContainer) => new RequestSubscribeStep(idContainer, subscribeOperation, successMessagBuilderFactory, sirenaNotFoundMessageBuilderFactory);
  }
}