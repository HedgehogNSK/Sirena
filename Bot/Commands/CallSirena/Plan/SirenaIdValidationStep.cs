using Hedgey.Localization;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class SirenaIdValidationStep : CommandStep
{
  private readonly NullableContainer<ObjectId> idContainer;
  private readonly ILocalizationProvider localizationProvider;

  public SirenaIdValidationStep(Container<IRequestContext> contextContainer
    , ILocalizationProvider localizationProvider
    , NullableContainer<ObjectId> idContainer)
  : base(contextContainer)
  {
    this.idContainer = idContainer;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make()
  {
    var chatId = Context.GetTargetChatId();
    var param = Context.GetArgsString();
    var info = Context.GetCultureInfo();

    Report report;
    if (!ObjectId.TryParse(param, out ObjectId sirenaId))
    {
      MessageBuilder builder = new StringNotIdMessageBuilder(chatId,info, localizationProvider, param);
      report = new Report(Result.Wait, builder);
    }
    else
    {
      idContainer.Set(sirenaId);
      report = new Report(Result.Success);
    }
    return Observable.Return(report);
  }
}