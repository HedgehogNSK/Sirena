using Hedgey.Sirena.Database;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ConfirmationRemoveSirenaStep : DeleteSirenaStep
{
  public ConfirmationRemoveSirenaStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer)
   : base(contextContainer, sirenaContainer)
  {
  }

  public override IObservable<Report> Make()
  {
    Report report;
    var param = Context.GetArgsString();
    if (!bool.TryParse(param, out bool value))
    {
      long chatId = Context.GetTargetChatId();
      var messageBuilder = new ConfirmRemoveSirenaMessageBuilder(chatId, sirenaContainer.Object);
      report = new Report(Result.Wait, messageBuilder);
    }
    else
    {
      report = new Report(value ? Result.Success : Result.Canceled);
    }

    return Observable.Return(report);
  }
}