using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestSubscribeStep(NullableContainer<ulong> sirenaIdContainter
  , ISubscribeToSirenaOperation subscribeOperation
  , IFactory<IRequestContext, SirenRepresentation, SuccesfulSubscriptionMessageBuilder> successMessagBuilderFactory
  , IFactory<IRequestContext, ulong, ISendMessageBuilder> sirenaNotFoundMessageBuilderFactory)
  : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var sid = sirenaIdContainter.Get();
    var uid = context.GetUser().Id;

    var request = subscribeOperation.Subscribe(uid, sid).Publish().RefCount(2);
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
  , IFactory<IRequestContext, ulong, ISendMessageBuilder> sirenaNotFoundMessageBuilderFactory)
  : IFactory<NullableContainer<ulong>, RequestSubscribeStep>
  {
    public RequestSubscribeStep Create(NullableContainer<ulong> idContainer) => new RequestSubscribeStep(idContainer, subscribeOperation, successMessagBuilderFactory, sirenaNotFoundMessageBuilderFactory);
  }
}