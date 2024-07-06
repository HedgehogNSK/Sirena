using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaExistensValidationStep : CommandStep
{
  private readonly NullableContainer<ObjectId> idContainer;
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly ILocalizationProvider localizationProvider;

  public SirenaExistensValidationStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> idContainer, NullableContainer<SirenRepresentation> sirenaContainer
  , IFindSirenaOperation findSirenaOperation, ILocalizationProvider localizationProvider)
  : base(contextContainer)
  {
    this.idContainer = idContainer;
    this.sirenaContainer = sirenaContainer;
    this.findSirenaOperation = findSirenaOperation;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make()
  {
    var sirenaId = idContainer.Get();
    return findSirenaOperation.Find(sirenaId).Select(CreateReport);

  }

  private Report CreateReport(SirenRepresentation representation)
  {
    var chatId = Context.GetTargetChatId();
    var info = Context.GetCultureInfo();
    var sirenaId = idContainer.Get();
    if (representation==null)
      return new Report(Result.Wait, new NoSirenaMessageBuilder(chatId,info, localizationProvider, sirenaId));
    sirenaContainer.Set(representation);
    return new Report(Result.Success);
  }
}
