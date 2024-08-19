using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class RequestSubscribeStep : CommandStep
{
  private readonly NullableContainer<ObjectId> sirenaIdContainter;
  private readonly ISubscribeToSirenaOperation subscribeOperation;
  private readonly ILocalizationProvider localizationProvider;

  public RequestSubscribeStep(NullableContainer<ObjectId> sirenaIdContainter
  , ISubscribeToSirenaOperation subscribeOperation, ILocalizationProvider localizationProvider)
  {
    this.sirenaIdContainter = sirenaIdContainter;
    this.subscribeOperation = subscribeOperation;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var id = sirenaIdContainter.Get();
    var uid = context.GetUser().Id;

    var request = subscribeOperation.Subscribe(uid, id).Publish().RefCount();
    var fail = request.Where(x => x == null).Select(_ => CreateReportNotFound());
    var success = request.Where(x => x != null).Select(CreateSuccesfulReport);
    return success.Merge(fail);

    Report CreateSuccesfulReport(SirenRepresentation representation)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      MessageBuilder meesage = new SuccesfulSubscriptionMessageBuilder(chatId, info, localizationProvider, representation);
      return new Report(Result.Success, meesage);
    }
    Report CreateReportNotFound()
    {
      var id = sirenaIdContainter.Get();
      var info = context.GetCultureInfo();
      var chatId = context.GetTargetChatId();
      return new(Result.Wait, new SirenaNotFoundMessageBuilder(chatId, info, localizationProvider, id));
    }
  }
}