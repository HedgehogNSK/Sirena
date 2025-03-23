using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public sealed class DeclineRequestValidateSirenaIdStep(
    NullableContainer<ulong> sirenaIdContainter)
  : ValidateSirenaIdBaseStep(sirenaIdContainter)
{
  protected override Report EmptyKeyReport(IRequestContext context, string key)
    => new Report(Result.Wait, Fallback: new FallbackRequestContext(context, RequestsCommand.NAME));

  protected override Report IsNotIdReport(IRequestContext context, string key)
    => new Report(Result.Wait, Fallback: new FallbackRequestContext(context, RequestsCommand.NAME));

  protected override Report SuccessReport(IRequestContext context, string key)
    => new Report(Result.Success);

  public sealed class Factory : IFactory<NullableContainer<ulong>, DeclineRequestValidateSirenaIdStep>
  {
    public DeclineRequestValidateSirenaIdStep Create(NullableContainer<ulong> container)
     => new DeclineRequestValidateSirenaIdStep(container);
  }
}