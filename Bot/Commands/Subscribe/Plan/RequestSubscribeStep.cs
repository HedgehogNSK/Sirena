using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using MongoDB.Driver;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestSubscribeStep(NullableContainer<ulong> sirenaIdContainter
  , ISubscribeToSirenaOperation subscribeOperation
  , IFactory<IRequestContext, SirenaData, SuccesfulSubscriptionMessageBuilder> successMessagBuilderFactory
  , IFactory<IRequestContext, ulong, ISendMessageBuilder> sirenaNotFoundMessageBuilderFactory
  , IFactory<IRequestContext, IEditMessageReplyMarkupBuilder> switchButtonFactory )
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

    Report CreateSuccesfulReport(SirenaData representation)
    {
      var meesage = successMessagBuilderFactory.Create(context, representation);
      var editButton = context.GetMessage().From.IsBot ? switchButtonFactory.Create(context) : null;

      return new Report(Result.Success, meesage, EditMessageReplyMarkupBuilder: editButton);
    }
    Report CreateReportNotFound()
    {
      var id = sirenaIdContainter.Get();
      return new(Result.Wait, sirenaNotFoundMessageBuilderFactory.Create(context, id));
    }
  }
  public class Factory(ISubscribeToSirenaOperation subscribeOperation
  , IFactory<IRequestContext, SirenaData, SuccesfulSubscriptionMessageBuilder> successMessagBuilderFactory
  , IFactory<IRequestContext, ulong, ISendMessageBuilder> sirenaNotFoundMessageBuilderFactory
  , IFactory<IRequestContext, IEditMessageReplyMarkupBuilder> switchButtonFactory)
  : IFactory<NullableContainer<ulong>, RequestSubscribeStep>
  {
    public RequestSubscribeStep Create(NullableContainer<ulong> idContainer) => new RequestSubscribeStep(idContainer, subscribeOperation, successMessagBuilderFactory, sirenaNotFoundMessageBuilderFactory,switchButtonFactory);
  }
}