using Hedgey.Blendflake;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Utilities;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class TryUnsubscribeStep : CommandStep
{
  private readonly NullableContainer<ulong> idContainer;
  private readonly IUnsubscribeSirenaOperation unsubscribeOperation;
  private readonly IFactory<IRequestContext, string, bool, IMessageBuilder> messageBuilderFactory;

  public TryUnsubscribeStep(NullableContainer<ulong> idContainer
  , IUnsubscribeSirenaOperation unsubscribeOperation
  , IFactory<IRequestContext, string, bool, IMessageBuilder> messageBuilderFactory)
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
      string hash = NotBase64URL.From(id);
      hash = HashUtilities.Shortify(hash);
      IMessageBuilder builder = messageBuilderFactory.Create(context, hash, isSuccess);
      return new Report(result, builder);
    }
  }

  public class Factory(IUnsubscribeSirenaOperation unsubscribeOperation
  , IFactory<IRequestContext, string, bool, IMessageBuilder> messageBuilderFactory) : IFactory<NullableContainer<ulong>, TryUnsubscribeStep>
  {
    public TryUnsubscribeStep Create(NullableContainer<ulong> idContainer)
    => new TryUnsubscribeStep(idContainer, unsubscribeOperation, messageBuilderFactory);
  }
}