using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using MongoDB.Driver.Linq;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestFindSirenaStep(IFindSirenaOperation findSirenaOperation
  , IGetUserInformation getUserInformation
  , ILocalizationProvider localizationProvider) : CommandStep
{
  private readonly IFindSirenaOperation findSirenaOperation = findSirenaOperation;
  private readonly IGetUserInformation getUserInformation = getUserInformation;
  private readonly ILocalizationProvider localizationProvider = localizationProvider;

  public override IObservable<Report> Make(IRequestContext context)
  {
    var searchKey = context.GetArgsString();
    var findObservable = findSirenaOperation.Find(searchKey)
      .DelaySubscription(TimeSpan.FromMicroseconds(1)) //Crunch to prevent early execution
      .Publish()
      .RefCount();

    IObservable<Report> emptyList = findObservable.Where(_sirenas => !_sirenas.Any())
      .Select(_ => NoSirenaReport(context));

    IObservable<Report> succesful = findObservable
      .SelectMany(x => x)
      .SelectMany(GetOwnerNickname)
      .ToArray() //If no element was emitted, ToArray returns empty array
      .Where(x => x.Any())
      .Select(CreateReport);

    return succesful.Merge(emptyList);

    Report CreateReport((SirenaData, string)[] source)
    {
      var info = context.GetCultureInfo();
      var chatId = context.GetTargetChatId();
      MessageBuilder builder = new ListSirenaMessageBuilder(chatId, info, localizationProvider, source);
      return new Report(Result.Success, builder);
    }
  }
  private IObservable<(SirenaData sirena, string ownerName)> GetOwnerNickname(SirenaData sirena)
    => getUserInformation.GetNickname(sirena.OwnerId).Select(x => (sirena, x));

  private Report NoSirenaReport(IRequestContext context)
  {
    var chatId = context.GetTargetChatId();
    var info = context.GetCultureInfo();
    string title = context.GetArgsString();
    MessageBuilder builder = new NoSirenaWithSuchTitleMessageBuilder(chatId, info, localizationProvider, title);
    return new Report(Result.Wait, builder);
  }
}