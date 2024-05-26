using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateTitleCreateSirenaStep(Container<IRequestContext> contextContainer
  , CreateSirenaStep.Buffer buffer)
: CreateSirenaStep(contextContainer, buffer)
{
  private const int TITLE_MAX_LENGHT = 256;
  private const int TITLE_MIN_LENGHT = 3;

  public override IObservable<Report> Make()
  {
    string sirenaTitle = contextContainer.Object.GetArgsString().Trim();

    Report report;
    if (string.IsNullOrEmpty(sirenaTitle) || sirenaTitle.Length < TITLE_MIN_LENGHT)
    {
      buffer.MessageBuilder.IsTitleValid(false, TITLE_MIN_LENGHT, TITLE_MAX_LENGHT);
      report = new Report(Result.Wait, buffer.MessageBuilder);
    }
    else
    {
      buffer.SirenaTitle = sirenaTitle;
      buffer.MessageBuilder.IsTitleValid(true, TITLE_MIN_LENGHT, TITLE_MAX_LENGHT);
      report = new(Result.Success, null);
    }
    return Observable.Return(report);
  }
}