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

  public RequestFindSirenaStep(Container<IRequestContext> contextContainer
  , IFindSirenaOperation findSirenaOperation, TelegramBot bot
  , ILocalizationProvider localizationProvider)
    : base(contextContainer)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.bot = bot;
    this.localizationProvider = localizationProvider;
  }
  public override IObservable<Report> Make()
  {
    var searchKey = Context.GetArgsString();
    var findObservable = findSirenaOperation.Find(searchKey)
      .DelaySubscription(TimeSpan.FromMicroseconds(1)) //Crunch to prevent early execution
      .Publish()
      .RefCount();

    IObservable<Report> emptyList = findObservable.Where(_sirenas => !_sirenas.Any())
      .Select(_ => NoSirenaReport());

    IObservable<Report> succesful = findObservable
      .SelectMany(x => x)
      .SelectMany(GetOwnerNickname)
      .ToArray() //If no element was emitted, ToArray returns empty array
      .Where(x => x.Any())
      .Select(CreateReport);

    return succesful.Merge(emptyList);
  }
  private IObservable<(SirenRepresentation sirena, string ownerName)> GetOwnerNickname(SirenRepresentation sirena)
    => Observable.FromAsync(() => BotTools.GetDisplayName(bot, sirena.OwnerId)).Select(x => (sirena, x));

  private Report NoSirenaReport()
  {
    var chatId = Context.GetTargetChatId();
    var info = Context.GetCultureInfo();
    string key = Context.GetArgsString();
    MessageBuilder builder = new NoSirenaMessageBuilder(chatId,info, localizationProvider, key);
    return new Report(Result.Wait, builder);
  }

  private Report CreateReport((SirenRepresentation, string)[] source)
  {
    var info = Context.GetCultureInfo();
    var chatId = Context.GetTargetChatId();
    MessageBuilder builder = new ListSirenaMessageBuilder(chatId,info, localizationProvider, source);
    return new Report(Result.Success, builder);
  }
}