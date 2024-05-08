using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class GetRequestsListCommand : AbstractBotCommmand
{
  const string NAME = "requests";
  const string DESCRIPTION = "Display a list of requests for permission to launch a sirena.";
  private const string noRequestsMessage = "There are no requests for delegation of rights";
  private const string noSirenaMessage = "You don't have any sirenas yet.";
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly TelegramBot bot;
  private readonly FacadeMongoDBRequests requests;

  public GetRequestsListCommand(IMongoDatabase db, FacadeMongoDBRequests requests, TelegramBot bot)
  : base(NAME, DESCRIPTION)
  {
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    this.bot = bot;
    this.requests = requests;
  }

  public override async void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
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
      messageText = noSirenaMessage;
    }
    else
    {
      var requestsList = from siren in userSirensWithRequests
                         from request in siren.Requests
                         select new RequestInfo
                         {
                           SirenId = siren.Id,
                           Title = siren.Title,
                           UserId = request.UID,
                           Message = request.Message,
                         };

      messageText = await CreateMessageText(requestsList);
    }
    Program.messageSender.Send(chatId, messageText);
  }

  private async Task<string> CreateMessageText(IEnumerable<RequestInfo> requestsList)
  {
    if (!requestsList.Any())
    {
      return noRequestsMessage;
    }
    StringBuilder builder = new StringBuilder("Reuqests list\n");
    int number = 1;
    foreach (var request in requestsList)
    {
      var chat = await bot.GetChatByUID(request.UserId);
      var username = chat?.GetUsername()?? "Ghost";
      builder.AppendLine().Append(number).Append(". User *")
        .Append(username)
        .Append('|')
        .Append(request.UserId)
        .Append("* is asking for access to \n ")
        .Append("sirena ")
        .Append(request)
        .AppendLine();
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
    public ObjectId SirenId { get; set; }
    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Title { get; internal set; } = string.Empty;

    public override string ToString()
    {
      return "*\""+Title+"\"* : _"+ SirenId+'_';
    }
  }
}