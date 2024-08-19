using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateTitleCommandStep : CommandStep
{
  public const int TITLE_MAX_LENGHT = 256;
  public const int TITLE_MIN_LENGHT = 3;
  private readonly Container<CreateMessageBuilder> createMessageBuilder;
  private readonly NullableContainer<string> titleContainer;

  public ValidateTitleCommandStep(
     Container<CreateMessageBuilder> createMessageBuilder
  , NullableContainer<string> titleContainer) : base()
  {
    this.createMessageBuilder = createMessageBuilder;
    this.titleContainer = titleContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    string sirenaTitle = context.GetArgsString().Trim();
    var builder = createMessageBuilder.Object;
    Report report;
    if (string.IsNullOrEmpty(sirenaTitle) || sirenaTitle.Length < TITLE_MIN_LENGHT)
    {
      builder.IsTitleValid(false);
      report = new Report(Result.Wait, builder);
    }
    else
    {
      titleContainer.Set(sirenaTitle);
      builder.IsTitleValid(true);
      report = new(Result.Success, null);
    }
    return Observable.Return(report);
  }
}