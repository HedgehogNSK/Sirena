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

  public GetSirenaInfoStep(NullableContainer<ObjectId> sirenaIdContainter, IFindSirenaOperation findSirena, ILocalizationProvider localizationProvider)
  {
    this.sirenaIdContainter = sirenaIdContainter;
    this.findSirena = findSirena;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    return findSirena.Find(sirenaIdContainter.Get()).Select(CreateReport);

    Report CreateReport(SirenRepresentation representation)
    {
      var chatId = context.GetTargetChatId();
      var uid = context.GetUser().Id;
      var info = context.GetCultureInfo();

      Result result = representation == null ? Result.Canceled : Result.Success;
      MessageBuilder builder = representation == null ?
          new SirenaNotFoundMessageBuilder(chatId, info, localizationProvider, sirenaIdContainter.Get())
          : new SirenaInfoMessageBuilder(chatId, info, localizationProvider, uid, representation);

      return new Report(result, builder);
    }
  }
}