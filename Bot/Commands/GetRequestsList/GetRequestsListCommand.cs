using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Globalization;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class GetRequestsListCommand : AbstractBotCommmand
{
  public const string NAME = "requests";
  public const string DESCRIPTION = "Display a list of requests for permission to launch a sirena.";
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly TelegramBot bot;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;
  public GetRequestsListCommand(IMongoDatabase db, TelegramBot bot, ILocalizationProvider localizationProvider, IMessageSender messageSender)
  : base(NAME, DESCRIPTION)
  {
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    this.bot = bot;
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }
  public override async void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    CultureInfo info = context.GetCultureInfo();

    var filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.OwnerId, uid)
               & filterBuilder.SizeGt(s => s.Requests, 0);
    var projection = Builders<SirenRepresentation>.Projection
        .Include(x => x.Id)
        .Include(x => x.Title)
        .Include(x => x.Requests);
    var userSirensWithRequests = await sirens.Find(filter).Project<SirenRepresentation>(projection).ToListAsync();
    string messageText;
    if (userSirensWithRequests.Count == 0)
    {
      messageText = localizationProvider.Get("command.get_requests.no_requests", info);
    }
    else
    {
      var requestsList = from siren in userSirensWithRequests
                         from request in siren.Requests
                         select new RequestInfo
                         {
                           SirenId = siren.Sid,
                           Title = siren.Title,
                           UserId = request.UID,
                           Message = request.Message,
                         };

      messageText = await CreateMessageText(requestsList, info);
    }
    messageSender.Send(chatId, messageText);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(localizationProvider);
  }

  private async Task<string> CreateMessageText(IEnumerable<RequestInfo> requestsList, CultureInfo info)
  {
    string header = localizationProvider.Get("command.get_requests.header", info);
    StringBuilder builder = new StringBuilder(header);
    int number = 1;
    string requestMessageTemplate = localizationProvider.Get("command.get_requests.request_template", info);
    foreach (var request in requestsList)
    {
      var chat = await bot.GetChatByUID(request.UserId);
      var username = chat?.Username ?? localizationProvider.Get("miscellaneous.user_ghost", info);

      builder.AppendLine().Append(number)
      .AppendFormat(requestMessageTemplate, username, request.UserId, request);

      if (!string.IsNullOrEmpty(request.Message))
      {
        builder.Append('\"')
          .Append(request.Message)
          .Append('\"')
          .AppendLine();
      }
      ++number;
    }
    return builder.ToString();
  }

  public class RequestInfo
  {
    public ulong SirenId { get; set; }
    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Title { get; internal set; } = string.Empty;

    public override string ToString()
    {
      return "*\"" + Title + "\"* : `" + SirenId + '`';
    }
  }
}