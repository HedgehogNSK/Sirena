using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class TryUnsubscribeStep : CommandStep
{
  private readonly NullableContainer<ObjectId> idContainer;
  private readonly IUnsubscribeSirenaOperation unsubscribeOperation;
  private readonly IFactory<IRequestContext, ObjectId,bool, IMessageBuilder> messageBuilderFactory;

  public TryUnsubscribeStep(NullableContainer<ObjectId> idContainer
  , IUnsubscribeSirenaOperation unsubscribeOperation
  , IFactory<IRequestContext, ObjectId, bool, IMessageBuilder> messageBuilderFactory)
  {
    this.idContainer = idContainer;
    this.unsubscribeOperation = unsubscribeOperation;
    this.messageBuilderFactory = messageBuilderFactory;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var id = idContainer.Get();
    var uid = context.GetUser().Id;
    return unsubscribeOperation.Unsubscribe(uid, id).Select(CreateReport);

    Report CreateReport(bool isSuccess)
    {
      var info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      Result result = isSuccess ? Result.Success : Result.Canceled;
      IMessageBuilder builder = messageBuilderFactory.Create(context,id, isSuccess);
      return new Report(result, builder);
    }
  }
  
  public class Factory(IUnsubscribeSirenaOperation unsubscribeOperation, IFactory<IRequestContext, ObjectId, bool, IMessageBuilder> messageBuilderFactory) : IFactory<NullableContainer<ObjectId>, TryUnsubscribeStep>
  {
    public TryUnsubscribeStep Create(NullableContainer<ObjectId> idContainer)
    => new TryUnsubscribeStep(idContainer,unsubscribeOperation, messageBuilderFactory);
  }
}