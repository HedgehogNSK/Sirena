using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ConfirmationRemoveSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFactory<IRequestContext, SirenRepresentation, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory)
  : DeleteSirenaStep(sirenaContainer)
{
  private readonly IFactory<IRequestContext, SirenRepresentation, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory = messageBuilderFactory;
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

  public class Factory(IFactory<IRequestContext, SirenRepresentation, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<SirenRepresentation>, ConfirmationRemoveSirenaStep>
  {
    public ConfirmationRemoveSirenaStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
      => new ConfirmationRemoveSirenaStep(sirenaContainer, messageBuilderFactory);
  }
}