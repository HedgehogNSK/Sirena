using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestsValidateSirenaIdStep(NullableContainer<ulong> sirenaIdContainter)
  : ValidateSirenaIdBaseStep(sirenaIdContainter)
{
  private static Report GetFallbackReport(IRequestContext context)
  {
    FallbackRequestContext fallback = new FallbackRequestContext(context, RequestsCommand.NAME);
    return new Report(fallback);
  }
  protected override Report EmptyKeyReport(IRequestContext context, string key)
    => GetFallbackReport(context);

  protected override Report IsNotIdReport(IRequestContext context, string key)
    => GetFallbackReport(context);

  protected override Report SuccessReport(IRequestContext context, string key)
    => new Report(Result.Success);

  public class Factory: IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep>
  {
    RequestsValidateSirenaIdStep  IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep>.Create(
      NullableContainer<ulong> sirenaContainer
      )
      => new RequestsValidateSirenaIdStep(sirenaContainer);
  }
}