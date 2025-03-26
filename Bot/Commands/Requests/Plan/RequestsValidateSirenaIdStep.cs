using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestsValidateSirenaIdStep(NullableContainer<ulong> sirenaIdContainter
  , ISendMessageBuilder messageBuilder) 
  : ValidateSirenaIdBaseStep(sirenaIdContainter)
{

  protected override Report EmptyKeyReport(IRequestContext context, string key) 
    => new Report(Result.Wait, messageBuilder);

  protected override Report IsNotIdReport(IRequestContext context, string key) 
    => new Report(Result.Wait, messageBuilder);

  protected override Report SuccessReport(IRequestContext context, string key) 
    => new Report(Result.Success);

  public class Factory: IFactory<NullableContainer<ulong>, ISendMessageBuilder, RequestsValidateSirenaIdStep>
  {
    RequestsValidateSirenaIdStep  IFactory<NullableContainer<ulong>, ISendMessageBuilder, RequestsValidateSirenaIdStep>.Create(
      NullableContainer<ulong> sirenaContainer, ISendMessageBuilder messageBuilder
      )
      => new RequestsValidateSirenaIdStep(sirenaContainer, messageBuilder);
  }
}