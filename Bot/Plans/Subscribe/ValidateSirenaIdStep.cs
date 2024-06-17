using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateSirenaIdStep : CommandStep
{
  private readonly NullableContainer<ObjectId> sirenaIdContainter;

  public ValidateSirenaIdStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> sirenaIdContainter)
   : base(contextContainer)
  {
    this.sirenaIdContainter = sirenaIdContainter;
  }

  public override IObservable<Report> Make()
  {
    var key = Context.GetArgsString();
    var info = Context.GetCultureInfo();
    long chatId = Context.GetTargetChatId();
    Result result = Result.Success;
    MessageBuilder? messageBuilder = null;
    if (string.IsNullOrEmpty(key) || !ObjectId.TryParse(key, out var id))
    {
      result = Result.Wait;
      messageBuilder = new AskSirenaIdMessageBuilder(chatId,info, Program.LocalizationProvider);
    }
    else if (id != default)
      sirenaIdContainter.Set(id);

    return Observable.Return(new Report(result, messageBuilder));
  }
}