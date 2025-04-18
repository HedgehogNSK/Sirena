using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SirenaStateValidationStep(NullableContainer<SirenaData> sirenaContainer
  , IFactory<IRequestContext, SirenaData, ISendMessageBuilder> messageBuilderFactory)
  : CommandStep
{
  static public readonly TimeSpan allowedCallPeriod = TimeSpan.FromMinutes(1);

  public override IObservable<Report> Make(IRequestContext context)
  {
    long uid = context.GetUser().Id;

    SirenaData sirena = sirenaContainer.Get();
    Report report;
    if (sirena.CanBeCalledBy(uid) && IsReadyToCall(sirena))
    {
      report = new Report(Result.Success, null);
    }
    else
      report = new Report(Result.Canceled, messageBuilderFactory.Create(context, sirena));
    return Observable.Return(report);
  }

  private static bool IsReadyToCall(SirenaData sirena)
  {
    if (sirena.LastCall == null)
      return true;

    var timePassed = DateTimeOffset.UtcNow - sirena.LastCall.Date;
    return timePassed > allowedCallPeriod;
  }
  public class Factory(IFactory<IRequestContext, SirenaData, ISendMessageBuilder> messageBuilderFactory)
     : IFactory<NullableContainer<SirenaData>, SirenaStateValidationStep>
  {
    public SirenaStateValidationStep Create(NullableContainer<SirenaData> sirenaContainer)
      => new SirenaStateValidationStep(sirenaContainer, messageBuilderFactory);
  }
}