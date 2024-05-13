using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateIdSubscribeStep : CommandStep
{
  private readonly NullableContainer<ObjectId> sirenaIdContainter;

  public ValidateIdSubscribeStep(Container<IRequestContext> contextContainer, NullableContainer<ObjectId> sirenaIdContainter)
   : base(contextContainer)
  {
    this.sirenaIdContainter = sirenaIdContainter;
  }

  public override IObservable<Report> Make()
  {
    var context = contextContainer.Object;
    var key = context.GetArgsString();
    long chatId = context.GetTargetChatId();
    Result result = Result.Success;
    MessageBuilder? messageBuilder = null;
    if (string.IsNullOrEmpty(key) || !ObjectId.TryParse(key, out var id))
    {
      result = Result.Wait;
      messageBuilder = new AskSirenaIdMessageBuilder(chatId);
    }
    else if (id != default)
      sirenaIdContainter.Set(id);

    return Observable.Return(new Report(result, messageBuilder));
  }
}