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

  public SirenaExistensValidationStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> idContainer, NullableContainer<SirenRepresentation> sirenaContainer
  , IFindSirenaOperation findSirenaOperation) 
  : base(contextContainer)
  {
    this.idContainer = idContainer;
    this.sirenaContainer = sirenaContainer;
    this.findSirenaOperation = findSirenaOperation;
  }

  public override IObservable<Report> Make()
  {
    var sirenaId = idContainer.Get();
    return findSirenaOperation.Find(sirenaId).Select(CreateReport);

  }

  private Report CreateReport(SirenRepresentation representation)
  {
    var chatId = Context.GetTargetChatId();
    var sirenaId = idContainer.Get();
    if (representation==null)
      return new Report(Result.Wait, new NoSirenaMessageBuilder(chatId, sirenaId));
    sirenaContainer.Set(representation);
    return new Report(Result.Success);
  }
}
