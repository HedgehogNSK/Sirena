using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using MongoDB.Driver.Linq;
using RxTelegram.Bot;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Hedgey.Sirena.Bot;

public class RequestFindSirenaStep : CommandStep
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly TelegramBot bot;
  public RequestFindSirenaStep(Container<IRequestContext> contextContainer
  , IFindSirenaOperation findSirenaOperation, TelegramBot bot)
    : base(contextContainer)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.bot = bot;
  }

  public override IObservable<Report> Make()
  {
    var searchKey = contextContainer.Object.GetArgsString();
    var findObservable = findSirenaOperation.Find(searchKey).Publish().RefCount();

    IObservable<Report> emptyList = findObservable.Where(_sirenas=> !_sirenas.Any())
        .Select(_ => NoSirenaReport());

    IObservable<Report> succesful = findObservable
        .SelectMany(x => x)
        .SelectMany(GetOwnerNickname)
        .ToArray() //If no element was emitted, ToArray returns empty array
        .Where(x=>x.Any())
        .Select(CreateReport);

    return succesful.Merge(emptyList);
  }

  private IObservable<(SirenRepresentation sirena,string ownerName)> GetOwnerNickname(SirenRepresentation sirena)
    => BotTools.GetUsername(bot, sirena.OwnerId).ToObservable().Select(x=> (sirena, x));

  private Report NoSirenaReport()
  {
    var chatId = contextContainer.Object.GetTargetChatId();
    string key = contextContainer.Object.GetArgsString();
    MessageBuilder builder = new NoSirenaMessageBuilder(chatId, key);
    return new Report(Result.Wait, builder);
  }

  private Report CreateReport((SirenRepresentation, string)[] source)
  {
    var chatId = contextContainer.Object.GetTargetChatId();
    MessageBuilder builder = new ListSirenaMessageBuilder(chatId, source);
    return new Report(Result.Success, builder);
  }
}