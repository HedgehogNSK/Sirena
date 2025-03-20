using Hedgey.Blendflake;
using Hedgey.Extensions;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ValidateSirenaIdStep2(NullableContainer<ulong> sirenaIdContainter
  , ISendMessageBuilder messageBuilder) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var key = context.GetArgsString().GetParameterByNumber(0);

    if (string.IsNullOrEmpty(key) || !HashUtilities.TryParse(key, out var id))
      return Observable.Return(new Report(Result.Wait, messageBuilder));

    sirenaIdContainter.Set(id);
    return Observable.Return(new Report(Result.Success));
  }

  public class Factory()
    : IFactory<NullableContainer<ulong>, ISendMessageBuilder, ValidateSirenaIdStep2>
  {
    public ValidateSirenaIdStep2 Create(NullableContainer<ulong> sirenaContainer, ISendMessageBuilder messageBuilder)
      => new ValidateSirenaIdStep2(sirenaContainer, messageBuilder);
  }
}