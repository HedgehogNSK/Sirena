using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateSearchParamFindSirenaStep : CommandStep
{
  public const int MIN_SIMBOLS = 3;
  public const int MAX_SIMBOLS = 200;

  public ValidateSearchParamFindSirenaStep(Container<IRequestContext> contextContainer)
   : base(contextContainer) { }

  public override IObservable<Report> Make()
  {
    var context = contextContainer.Object;
    var key = context.GetArgsString();
    long chatId = context.GetTargetChatId();
    Result result = Result.Success;
    MessageBuilder? messageBuilder = null;
    if (key.Length < MIN_SIMBOLS || key.Length > MAX_SIMBOLS)
    {
      result = Result.CanBeFixed;
      messageBuilder = new WrongSearchKeyFindSirenaMessageBuilder(chatId);
    }
    return Observable.Return(new Report(result, messageBuilder));
  }
}

