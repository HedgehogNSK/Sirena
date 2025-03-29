using Hedgey.Blendflake;
using Hedgey.Extensions;
using Hedgey.Telegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public abstract class ValidateSirenaIdBaseStep(NullableContainer<ulong> sirenaIdContainter)
: CommandStep
{
  private readonly NullableContainer<ulong> sirenaIdContainter = sirenaIdContainter;

  public override IObservable<Report> Make(IRequestContext context)
  {
    Report report;
    var key = context.GetArgsString().GetParameterByNumber(0);
    if (string.IsNullOrEmpty(key))
    {
      report = EmptyKeyReport(context, key);
      return Observable.Return(report);
    }
    if (!HashUtilities.TryParse(key, out var id))
    {
      report = IsNotIdReport(context, key);
      return Observable.Return(report);
    }

    sirenaIdContainter.Set(id);
    report = SuccessReport(context, key);
    return Observable.Return(report);
  }

  protected abstract Report SuccessReport(IRequestContext context, string key);
  protected abstract Report IsNotIdReport(IRequestContext context, string key);
  protected abstract Report EmptyKeyReport(IRequestContext context, string key);
}