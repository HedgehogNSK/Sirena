using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class ProcessParameterUnsubscribeStep(NullableContainer<ObjectId> idContainer
  , IGetUserRelatedSirenas getSubscriptions
  , IGetUserInformation getUserInformation
  , IFactory<IRequestContext, IEnumerable<SirenRepresentation>, IMessageBuilder> messageBuilderFactory)
   : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetTargetChatId();
    string param = context.GetArgsString().GetParameterByNumber(0);
    if (string.IsNullOrEmpty(param) || !ObjectId.TryParse(param, out ObjectId id))
    {
      return getSubscriptions.GetSubscriptions(uid).SelectMany(_sirenas => _sirenas)
        .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId)
            .Do(_nick => _sirena.OwnerNickname = _nick)
            .Select(_ => _sirena))
        .ToArray()
        .Select(CreateSubscriptionList);
    }
    idContainer.Set(id);
    Report report = new Report(Result.Success, null);
    return Observable.Return(report);

    Report CreateSubscriptionList(IEnumerable<SirenRepresentation> subscriptions)
    {
      var info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      IMessageBuilder builder = messageBuilderFactory.Create(context,subscriptions);
      return new Report(!subscriptions.Any() ? Result.Wait : Result.Canceled, builder);
    }
  }

  public class Factory(
  IGetUserRelatedSirenas getSubscriptions, IGetUserInformation getUserInformation
  , IFactory<IRequestContext, IEnumerable<SirenRepresentation>, IMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<ObjectId>, ProcessParameterUnsubscribeStep>
  {
    public ProcessParameterUnsubscribeStep Create(NullableContainer<ObjectId> idContainer)
    => new ProcessParameterUnsubscribeStep(idContainer, getSubscriptions, getUserInformation, messageBuilderFactory);
  }
}