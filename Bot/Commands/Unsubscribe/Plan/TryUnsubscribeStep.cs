using Hedgey.Blendflake;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Utilities;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class TryUnsubscribeStep(NullableContainer<ulong> idContainer
  , IUnsubscribeSirenaOperation unsubscribeOperation
  , IFactory<IRequestContext, string, bool, ISendMessageBuilder> messageBuilderFactory
  , IFactory<IRequestContext, IEditMessageReplyMarkupBuilder> switchButtonFactory) 
  : CommandStep
{
  private readonly NullableContainer<ulong> idContainer = idContainer;
  private readonly IUnsubscribeSirenaOperation unsubscribeOperation = unsubscribeOperation;
  private readonly IFactory<IRequestContext, string, bool, ISendMessageBuilder> messageBuilderFactory = messageBuilderFactory;
  private readonly IFactory<IRequestContext, IEditMessageReplyMarkupBuilder> switchButtonFactory = switchButtonFactory;

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
      ISendMessageBuilder builder = messageBuilderFactory.Create(context, hash, isSuccess);
      var editButton = context.GetMessage().From.IsBot ? switchButtonFactory.Create(context) : null;
      return new Report(result, builder, EditMessageReplyMarkupBuilder: editButton);
    }
  }

  public class Factory(IUnsubscribeSirenaOperation unsubscribeOperation
  , IFactory<IRequestContext, string, bool, ISendMessageBuilder> messageBuilderFactory
  , IFactory<IRequestContext, IEditMessageReplyMarkupBuilder> switchButtonFactory)
  : IFactory<NullableContainer<ulong>, TryUnsubscribeStep>
  {
    public TryUnsubscribeStep Create(NullableContainer<ulong> idContainer)
    => new TryUnsubscribeStep(idContainer, unsubscribeOperation, messageBuilderFactory, switchButtonFactory);
  }
}