using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Data;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class DeleteConcretteSirenaStep : DeleteSirenaStep
{
  private readonly IDeleteSirenaOperation sirenaDeleteOperation;

  public DeleteConcretteSirenaStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer
  , IDeleteSirenaOperation sirenaDeleteOperation)
   : base(contextContainer, sirenaContainer )
  {
    this.sirenaDeleteOperation = sirenaDeleteOperation;
  }

  public override IObservable<Report> Make()
  {
    var uid = Context.GetUser().Id;
    var sirenaId = sirenaContainer.Get().Id;
    return sirenaDeleteOperation.Delete(uid, sirenaId)
    .Select(CreateReport);
  }

  private Report CreateReport(SirenRepresentation deletedSirena)
  {
    var info = Context.GetCultureInfo();
    var builder = new SuccesfulDeleteMessageBuilder(Context.GetTargetChatId(),info, Program.LocalizationProvider, deletedSirena);
    return new Report(Result.Success, builder);
  }
}