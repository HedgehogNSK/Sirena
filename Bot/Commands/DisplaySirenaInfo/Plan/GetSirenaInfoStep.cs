using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class GetSirenaInfoStep : CommandStep
{
  private readonly NullableContainer<ObjectId> sirenaIdContainter;
  private readonly IFindSirenaOperation findSirena;
  private readonly ILocalizationProvider localizationProvider;

  public GetSirenaInfoStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> sirenaIdContainter, IFindSirenaOperation findSirena, ILocalizationProvider localizationProvider)
   : base(contextContainer)
  {
    this.sirenaIdContainter = sirenaIdContainter;
    this.findSirena = findSirena;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make()
  => findSirena.Find(sirenaIdContainter.Get()).Select(CreateReport);

  private Report CreateReport(SirenRepresentation representation)
  {
    var chatId = Context.GetTargetChatId();
    var uid = Context.GetUser().Id;
    var info = Context.GetCultureInfo();

    Result result = representation == null ? Result.Canceled : Result.Success;
    MessageBuilder builder = representation == null ?
        new SirenaNotFoundMessageBuilder(chatId, info, localizationProvider, sirenaIdContainter.Get())
        : new SirenaInfoMessageBuilder(chatId, info, localizationProvider, uid, representation);

    return new Report(result, builder);
  }
}