using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ConfirmationRemoveSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFactory<IRequestContext, SirenRepresentation, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory)
  : DeleteSirenaStep(sirenaContainer)
{
  private readonly IFactory<IRequestContext, SirenRepresentation, ConfirmRemoveSirenaMessageBuilder> messageBuilderFactory = messageBuilderFactory;

  public override IObservable<Report> Make(IRequestContext context)
  {
    Report report;
    var info = context.GetCultureInfo();
    var param = context.GetArgsString();
    if (!bool.TryParse(param, out bool value))
    {
      long chatId = context.GetTargetChatId();
      var messageBuilder = messageBuilderFactory.Create(context, sirenaContainer.Get());
      report = new Report(Result.Wait, messageBuilder);
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