using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Data;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class DeleteConcretteSirenaStep : DeleteSirenaStep
{
  private readonly IDeleteSirenaOperation sirenaDeleteOperation;
  private readonly ILocalizationProvider localizationProvider;

  public DeleteConcretteSirenaStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer
  , IDeleteSirenaOperation sirenaDeleteOperation,
ILocalizationProvider localizationProvider)
   : base(contextContainer, sirenaContainer)
  {
    this.sirenaDeleteOperation = sirenaDeleteOperation;
    this.localizationProvider = localizationProvider;
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
    var builder = new SuccesfulDeleteMessageBuilder(Context.GetTargetChatId(), info, localizationProvider, deletedSirena);
    return new Report(Result.Success, builder);
  }
}