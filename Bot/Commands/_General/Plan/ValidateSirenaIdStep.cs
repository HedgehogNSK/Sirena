using Hedgey.Blendflake;
using Hedgey.Extensions;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class ValidateSirenaIdStep(NullableContainer<ulong> sirenaIdContainter
  , IFactory<IRequestContext, ISendMessageBuilder> messageBuilderFactory) : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    var key = context.GetArgsString().GetParameterByNumber(0);

    if (string.IsNullOrEmpty(key) || !HashUtilities.TryParse(key, out var id))
      return Observable.Return(new Report(Result.Wait, messageBuilderFactory.Create(context)));

    sirenaIdContainter.Set(id);
    return Observable.Return(new Report(Result.Success));
  }

  public class Factory(IFactory<IRequestContext, ISendMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<ulong>, ValidateSirenaIdStep>
  {
    public ValidateSirenaIdStep Create(NullableContainer<ulong> sirenaContainer)
      => new ValidateSirenaIdStep(sirenaContainer, messageBuilderFactory);
  }
}