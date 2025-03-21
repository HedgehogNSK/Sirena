using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class ValidateTitleCommandStep : CommandStep
{
  public const int TITLE_MAX_LENGHT = 256;
  public const int TITLE_MIN_LENGHT = 3;
  private readonly CreateMessageBuilder messageBuilder;
  private readonly NullableContainer<string> titleContainer;

  public ValidateTitleCommandStep(
     CreateMessageBuilder messageBuilder
  , NullableContainer<string> titleContainer) : base()
  {
    this.messageBuilder = messageBuilder;
    this.titleContainer = titleContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    string sirenaTitle = context.GetArgsString().Trim();
    Report report;
    if (string.IsNullOrEmpty(sirenaTitle) || sirenaTitle.Length < TITLE_MIN_LENGHT)
    {
      messageBuilder.IsTitleValid(false);
      report = new Report(Result.Wait, messageBuilder);
    }
    else
    {
      titleContainer.Set(sirenaTitle);
      messageBuilder.IsTitleValid(true);
      report = new(Result.Success, null);
    }
    return Observable.Return(report);
  }
}