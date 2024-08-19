using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Driver.Linq;
using RxTelegram.Bot;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class RequestFindSirenaStep : CommandStep
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly TelegramBot bot;
  private readonly ILocalizationProvider localizationProvider;

  public RequestFindSirenaStep(IFindSirenaOperation findSirenaOperation, TelegramBot bot
  , ILocalizationProvider localizationProvider)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.bot = bot;
    this.localizationProvider = localizationProvider;
  }
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

    Report CreateReport((SirenRepresentation, string)[] source)
    {
      var info = context.GetCultureInfo();
      var chatId = context.GetTargetChatId();
      MessageBuilder builder = new ListSirenaMessageBuilder(chatId, info, localizationProvider, source);
      return new Report(Result.Success, builder);
    }
  }
  private IObservable<(SirenRepresentation sirena, string ownerName)> GetOwnerNickname(SirenRepresentation sirena)
    => Observable.FromAsync(() => BotTools.GetDisplayName(bot, sirena.OwnerId)).Select(x => (sirena, x));

  private Report NoSirenaReport(IRequestContext context)
  {
    var chatId = context.GetTargetChatId();
    var info = context.GetCultureInfo();
    string key = context.GetArgsString();
    MessageBuilder builder = new NoSirenaMessageBuilder(chatId, info, localizationProvider, key);
    return new Report(Result.Wait, builder);
  }
}