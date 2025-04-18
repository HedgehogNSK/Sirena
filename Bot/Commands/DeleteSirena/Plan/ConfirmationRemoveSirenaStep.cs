using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class ConfirmationRemoveSirenaStep(NullableContainer<SirenaData> sirenaContainer
  , IFactory<IRequestContext, SirenaData, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory)
  : DeleteSirenaStep(sirenaContainer)
{
  private readonly IFactory<IRequestContext, SirenaData, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory = messageBuilderFactory;
  bool warningIsShown = false;
  public override IObservable<Report> Make(IRequestContext context)
  {
    Report report;
    var param = context.GetArgsString();
    if (!warningIsShown)
    {
      var messageBuilder = messageBuilderFactory.Create(context, sirenaContainer.Get());
      report = new Report(Result.Wait, messageBuilder);
      warningIsShown = true;
    }
    else if(!bool.TryParse(param, out bool value)){
      report = new Report(Result.Wait);
    }
    else
    {
      report = new Report(value ? Result.Success : Result.Canceled);
    }

    return Observable.Return(report);
  }

  public class Factory(IFactory<IRequestContext, SirenaData, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<SirenaData>, ConfirmationRemoveSirenaStep>
  {
    public ConfirmationRemoveSirenaStep Create(NullableContainer<SirenaData> sirenaContainer)
      => new ConfirmationRemoveSirenaStep(sirenaContainer, messageBuilderFactory);
  }
}