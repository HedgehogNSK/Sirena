using Hedgey.Extensions;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Blendflake;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class ProcessParameterUnsubscribeStep(NullableContainer<ulong> idContainer
  , IGetUserRelatedSirenas getSubscriptions
  , IGetUserInformation getUserInformation
  , IFactory<IRequestContext, IEnumerable<SirenaData>, ISendMessageBuilder> messageBuilderFactory)
   : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetTargetChatId();
    var info = context.GetCultureInfo();
    string param = context.GetArgsString().GetParameterByNumber(0);
    if (string.IsNullOrEmpty(param) || !HashUtilities.TryParse(param, out var id))
    {
      return getSubscriptions.GetSubscriptions(uid).SelectMany(_sirenas => _sirenas)
        .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId, info)
            .Do(_nick => _sirena.OwnerNickname = _nick)
            .Select(_ => _sirena))
        .ToArray()
        .Select(CreateSubscriptionList);
    }
    idContainer.Set(id);
    Report report = new Report(Result.Success, null);
    return Observable.Return(report);

    Report CreateSubscriptionList(IEnumerable<SirenaData> subscriptions)
    {
      var info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      ISendMessageBuilder builder = messageBuilderFactory.Create(context, subscriptions);
      return new Report(!subscriptions.Any() ? Result.Wait : Result.Canceled, builder);
    }
  }

  public class Factory(
  IGetUserRelatedSirenas getSubscriptions, IGetUserInformation getUserInformation
  , IFactory<IRequestContext, IEnumerable<SirenaData>, ISendMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<ulong>, ProcessParameterUnsubscribeStep>
  {
    public ProcessParameterUnsubscribeStep Create(NullableContainer<ulong> idContainer)
    => new ProcessParameterUnsubscribeStep(idContainer, getSubscriptions, getUserInformation, messageBuilderFactory);
  }
}