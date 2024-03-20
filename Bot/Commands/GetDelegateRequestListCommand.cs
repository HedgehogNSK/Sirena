using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class GetDelegateRequestListCommand : BotCustomCommmand
{
  private readonly IMongoCollection<UserRepresentation> users;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly TelegramBot bot;
  private readonly FacadeMongoDBRequests requests;

  public GetDelegateRequestListCommand(string name, string description
  , IMongoDatabase db, FacadeMongoDBRequests requests, TelegramBot bot)
  : base(name, description)
  {
    users = db.GetCollection<UserRepresentation>("users");
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    this.bot = bot;
    this.requests = requests;
  }

  public override async void Execute(Message message)
  {
    long uid = message.From.Id;
    var filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.OwnerId, uid)
               & filterBuilder.SizeGt(s => s.Requests, 0);
    var projection = Builders<SirenRepresentation>.Projection
        .Include(x => x.Id)
        .Include(x => x.Title)
        .Include(x => x.Requests);
    var userSirensWithRequests = await sirens.Find(filter).Project<SirenRepresentation>(projection).ToListAsync();
    var requestsList = from siren in userSirensWithRequests
                       from request in siren.Requests
                       select new RequestInfo
                       {
                         SirenId = siren.Id,
                         Title = siren.Title,
                         UserId = request.UID,
                         Message = request.Message,
                       };

    var messageText = CreateMessageText(requestsList);
    Program.messageSender.Send(message.Chat.Id, messageText);
  }

  private string CreateMessageText(IEnumerable<RequestInfo> requestsList)
  {
    StringBuilder builder = new StringBuilder("Reuqests list\n");
    int number = 1;
    foreach (var request in requestsList)
    {
      builder.AppendLine().Append(number).Append(". User *")
        .Append(request.UserId)
        .Append("* is asking for access to:\n *\"")
        .Append(request.Title)
        .Append("\"* with id: ")
        .Append(request.SirenId)
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
  }
}