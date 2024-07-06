using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ProcessParameterUnsubscribeStep : CommandStep
{
  private readonly ILocalizationProvider localizationProvider;
  private readonly NullableContainer<ObjectId> IdContainer;
  private readonly IGetUserRelatedSirenas getSubscriptions;
  private readonly IGetUserInformation getUserInformation;
  public ProcessParameterUnsubscribeStep(Container<IRequestContext> contextContainer
  , NullableContainer<ObjectId> idContainer
  , IGetUserRelatedSirenas getSubscriptions
  , IGetUserInformation getUserInformation,
ILocalizationProvider localizationProvider)
   : base(contextContainer)
  {
    this.getSubscriptions = getSubscriptions;
    this.getUserInformation = getUserInformation;
    IdContainer = idContainer;
    this.localizationProvider = localizationProvider;
  }

  public override IObservable<Report> Make()
  {
    User botUser = Context.GetUser();
    long uid = botUser.Id;
    long chatId = Context.GetTargetChatId();
    string param = Context.GetArgsString().GetParameterByNumber(0);
    if (string.IsNullOrEmpty(param) || !ObjectId.TryParse(param, out ObjectId id))
    {
      return getSubscriptions.GetSubscriptions(uid).SelectMany(_sirenas => _sirenas)
        .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId).Select(_nick => (_sirena, _nick)))
        .ToArray()
        .Select(CreateSubscriptionList);
    }
    IdContainer.Set(id);
    Report report = new Report(Result.Success, null);
    return Observable.Return(report);
  }

  private Report CreateSubscriptionList(IEnumerable<(SirenRepresentation, string)> source)
  {
    var info = Context.GetCultureInfo();
    long chatId = Context.GetTargetChatId();
    MessageBuilder builder = new SubscriptionsMesssageBuilder(chatId,info, localizationProvider, source);
    return new Report(!source.Any() ? Result.Wait : Result.Canceled, builder);
  }
}
