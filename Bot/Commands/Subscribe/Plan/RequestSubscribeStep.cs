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

  public RequestSubscribeStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> sirenaIdContainter
  , ISubscribeToSirenaOperation subscribeOperation, ILocalizationProvider localizationProvider)
  : base(contextContainer)
  {
    this.sirenaIdContainter = sirenaIdContainter;
    this.subscribeOperation = subscribeOperation;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make()
  {
    var id = sirenaIdContainter.Get();
    var uid = Context.GetUser().Id;

    var request = subscribeOperation.Subscribe(uid, id).Publish().RefCount();
    var fail = request.Where(x => x == null).Select(_ => CreateReportNotFound());
    var success = request.Where(x => x != null).Select(CreateSuccesfulReport);
    return success.Merge(fail);
  }

  private Report CreateSuccesfulReport(SirenRepresentation representation)
  {
    var chatId = Context.GetTargetChatId();
    var info = Context.GetCultureInfo();
    MessageBuilder meesage = new SuccesfulSubscriptionMessageBuilder(chatId,info, localizationProvider , representation);
    return new Report(Result.Success, meesage);
  }

  private Report CreateReportNotFound()
  {
    var id = sirenaIdContainter.Get();
    var info = Context.GetCultureInfo();
    var chatId = Context.GetTargetChatId();
    return new(Result.Wait, new SirenaNotFoundMessageBuilder(chatId,info, localizationProvider, id));
  }
}