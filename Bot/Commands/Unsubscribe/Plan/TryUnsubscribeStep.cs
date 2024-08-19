using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using MongoDB.Bson;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class TryUnsubscribeStep : CommandStep
{
  private readonly NullableContainer<ObjectId> idContainer;
  private readonly IUnsubscribeSirenaOperation unsubscribeOperation;
  private readonly ILocalizationProvider localizationProvider;

  public TryUnsubscribeStep(NullableContainer<ObjectId> idContainer
  , IUnsubscribeSirenaOperation unsubscribeOperation
  , ILocalizationProvider localizationProvider)
  {
    this.idContainer = idContainer;
    this.unsubscribeOperation = unsubscribeOperation;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var id = idContainer.Get();
    var uid = context.GetUser().Id;
    return unsubscribeOperation.Unsubscribe(uid, id).Select(CreateReport);

    Report CreateReport(bool isSuccess)
    {
      var info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      Result result = isSuccess ? Result.Success : Result.Canceled;
      MessageBuilder builder = new UnsubscribeMessageBuilder(chatId, info, localizationProvider, isSuccess);
      return new Report(result, builder);
    }
  }
}