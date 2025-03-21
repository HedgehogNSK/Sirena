using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class DisplayCommandMenuStep(IGetUserRelatedSirenas userSirenas
,IGetUserInformation getUserInformation 
, IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder> messageBuilderFactory)
 : CommandStep
{
  public override IObservable<Report> Make(IRequestContext context)
  {
    string args = context.GetArgsString();
    Report report;
    if (string.IsNullOrWhiteSpace(args))
    {
      long uid = context.GetUser().Id;
    var info = context.GetCultureInfo();

      return userSirenas.GetSubscriptions(uid).SelectMany(_sirenas => _sirenas)
       .SelectMany(_sirena => getUserInformation.GetNickname(_sirena.OwnerId,info)
           .Do(_nick => _sirena.OwnerNickname = _nick)
           .Select(_ => _sirena))
       .ToArray()
       .Select(CreateSubscriptionList)
       .Catch((Exception ex) => Observable.Return(new Report(Result.Exception, null)));

      Report CreateSubscriptionList(IEnumerable<SirenRepresentation> sirens)
      {
        var notResponsibleSirens = sirens.Where(_siren => !_siren.CanBeCalledBy(uid)
           && !_siren.Requests.Any(_value => _value.UID == uid));
        var messageBuilder = messageBuilderFactory.Create(context, notResponsibleSirens);
        return new Report(Result.Wait, messageBuilder);
      }
    }
    else
      report = new Report(Result.Success, null);
    return Observable.Return(report);
  }
  public class Factory(
    IGetUserRelatedSirenas getSubscriptions, IGetUserInformation getUserInformation
  , IFactory<IRequestContext, IEnumerable<SirenRepresentation>, ISendMessageBuilder> messageBuilderFactory)
    : IFactory<DisplayCommandMenuStep>
  {
    public DisplayCommandMenuStep Create()
    => new DisplayCommandMenuStep(getSubscriptions, getUserInformation, messageBuilderFactory);
  }

}
