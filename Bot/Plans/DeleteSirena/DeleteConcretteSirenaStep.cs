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
   : base(contextContainer, sirenaContainer)
  {
    this.sirenaDeleteOperation = sirenaDeleteOperation;
  }

  public override IObservable<Report> Make()
  {
    var uid = contextContainer.Object.GetUser().Id;
    var sirenaId = sirenaContainer.Object.Id;
    return sirenaDeleteOperation.Delete(uid, sirenaId)
    .Select(CreateReport);
  }

  private Report CreateReport(SirenRepresentation deletedSirena)
  {
    var builder = new SuccesfulDeleteMessageBuilder(contextContainer.Object.GetTargetChatId(), deletedSirena);
    return new Report(Result.Success, builder);
  }
}