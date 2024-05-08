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
     var result = Result.Success;
    if (string.IsNullOrEmpty(sirenaTitle) || sirenaTitle.Length < TITLE_MIN_LENGHT)
    {
       result = Result.CanBeFixed;
    }
    buffer.SirenaTitle = sirenaTitle;
    buffer.MessageBuilder.IsTitleValid(result == Result.Success, TITLE_MIN_LENGHT, TITLE_MAX_LENGHT);
    Report report = new(result, buffer.MessageBuilder);
    return Observable.Return(report);
  }
}
