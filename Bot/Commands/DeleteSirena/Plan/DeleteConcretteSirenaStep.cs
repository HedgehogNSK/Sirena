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

  public DeleteConcretteSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IDeleteSirenaOperation sirenaDeleteOperation,
ILocalizationProvider localizationProvider)
   : base(sirenaContainer)
  {
    this.sirenaDeleteOperation = sirenaDeleteOperation;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var sirenaId = sirenaContainer.Get().Id;
    return sirenaDeleteOperation.Delete(uid, sirenaId)
    .Select(CreateReport);

    Report CreateReport(SirenRepresentation deletedSirena)
    {
      var info = context.GetCultureInfo();
      var builder = new SuccesfulDeleteMessageBuilder(context.GetTargetChatId()
      , info, localizationProvider, deletedSirena);
      return new Report(Result.Success, builder);
    }
  }
}