using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class GetSirenaInfoStep : CommandStep
{
  private readonly NullableContainer<ObjectId> sirenaIdContainter;
  private readonly IFindSirenaOperation findSirena;

  public GetSirenaInfoStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> sirenaIdContainter, IFindSirenaOperation findSirena)
   : base(contextContainer)
  {
    this.sirenaIdContainter = sirenaIdContainter;
    this.findSirena = findSirena;
  }

  public override IObservable<Report> Make()
  => findSirena.Find(sirenaIdContainter.Get()).Select(CreateReport);

  private Report CreateReport(SirenRepresentation representation)
  {
    var chatId = Context.GetTargetChatId();
    var uid = Context.GetUser().Id;

    Result result = representation == null ? Result.Canceled : Result.Success;
    MessageBuilder builder = representation == null ?
        new SirenaNotFoundMessageBuilder(chatId, sirenaIdContainter.Get())
        : new SirenaInfoMessageBuilder(chatId, uid, representation);

    return new Report(result, builder);
  }
}