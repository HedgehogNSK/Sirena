using System.Reactive.Linq;
using Hedgey.Localization;

namespace Hedgey.Sirena.Bot;

public class ValidateSearchParamFindSirenaStep : CommandStep
{
  public const int MIN_SIMBOLS = 3;
  public const int MAX_SIMBOLS = 200;
  private readonly ILocalizationProvider localizationProvider;

  public ValidateSearchParamFindSirenaStep(Container<IRequestContext> contextContainer
  , ILocalizationProvider localizationProvider)
   : base(contextContainer)
  {
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make()
  {
    var key = Context.GetArgsString();
    var info = Context.GetCultureInfo();
    long chatId = Context.GetTargetChatId();
    Result result = Result.Success;
    MessageBuilder? messageBuilder = null;
    if (key.Length < MIN_SIMBOLS || key.Length > MAX_SIMBOLS)
    {
      result = Result.Wait;
      messageBuilder = new WrongSearchKeyFindSirenaMessageBuilder(chatId,info, localizationProvider);
    }
    return Observable.Return(new Report(result, messageBuilder));
  }
}