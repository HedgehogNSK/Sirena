using Hedgey.Sirena.Bot.Operations;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class TryUnsubscribeStep : CommandStep
{
  private readonly NullableContainer<ObjectId> idContainer;
  private readonly IUnsubscribeSirenaOperation unsubscribeOperation;

  public TryUnsubscribeStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> idContainer, IUnsubscribeSirenaOperation unsubscribeOperation) 
  : base(contextContainer)
  {
    this.idContainer = idContainer;
    this.unsubscribeOperation = unsubscribeOperation;
  }

  public override IObservable<Report> Make()
  {
    var id = idContainer.Get();
    var uid = Context.GetUser().Id;
    return unsubscribeOperation.Unsubscribe(uid,id).Select(CreateReport);
  }

  private Report CreateReport(bool isSuccess)
  {
    var info = Context.GetCultureInfo();
    Result result =isSuccess? Result.Success : Result.Canceled;
    MessageBuilder builder = new UnsubscribeMessageBuilder(Context.GetTargetChatId(),info, Program.LocalizationProvider, isSuccess);
    return new Report(result, builder);
  }
}
