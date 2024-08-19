using System.Reactive.Linq;
using Hedgey.Localization;

namespace Hedgey.Sirena.Bot;

public class ValidateSearchParamFindSirenaStep : CommandStep
{
  public const int MIN_SIMBOLS = 3;
  public const int MAX_SIMBOLS = 200;
  private readonly ILocalizationProvider localizationProvider;

  public ValidateSearchParamFindSirenaStep(ILocalizationProvider localizationProvider)
  {
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var key = context.GetArgsString();
    var info = context.GetCultureInfo();
    long chatId = context.GetTargetChatId();
    Result result = Result.Success;
    MessageBuilder? messageBuilder = null;
    if (key.Length < MIN_SIMBOLS || key.Length > MAX_SIMBOLS)
    {
      result = Result.Wait;
      messageBuilder = new WrongSearchKeyFindSirenaMessageBuilder(chatId, info, localizationProvider);
    }
    return Observable.Return(new Report(result, messageBuilder));
  }
}