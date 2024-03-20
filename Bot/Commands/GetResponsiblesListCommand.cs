using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Chats;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class GetResponsiblesListCommand : BotCustomCommmand
{
  private readonly IMongoCollection<UserRepresentation> users;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly TelegramBot bot;
  private readonly FacadeMongoDBRequests requests;

  public GetResponsiblesListCommand(string name, string description
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
    string param = Extensions.TextTools.GetParameterByNumber(message.Text, 1);
    ObjectId sid = await requests.GetSirenaId(message.From.Id, param);
    if (sid == ObjectId.Empty) return;

    var filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.Id, sid);
    var projection = Builders<SirenRepresentation>.Projection
        .Include(x => x.Id)
        .Include(x => x.OwnerId)
        .Include(x => x.Title)
        .Include(x => x.Responsible);
    var sirena = await sirens.Find(filter)
        .Project<SirenRepresentation>(projection)
        .FirstOrDefaultAsync();

    string owner = string.Empty;
    string[] responsibles = [];
    if (sirena != null)
    {
      string username = await GetUsername(bot, sirena.OwnerId);
      owner = username;
      if (sirena.Responsible.Any())
      {
        responsibles = await GetResponsibleNames(sirena);
      }
    }

    var messageText = CreateMessageText(sirena, owner, responsibles);
    Program.messageSender.Send(message.Chat.Id, messageText);
  }

  private async Task<string[]> GetResponsibleNames(SirenRepresentation sirena)
  {
    string[] names = new string[sirena.Responsible.Length];
    for (int id = 0; id != sirena.Responsible.Length; ++id)
    {
      var username = await GetUsername(bot, sirena.Responsible[id]);
      names[id] = username;
    }

    return names;
  }

  private string CreateMessageText(SirenRepresentation? sirena, string owner, string[] responsibles)
  {
    if (sirena == null)
    {
      const string message = "Sirena wasn't found";
      return message;
    }

    StringBuilder builder = new StringBuilder("Responsibles for call sirena \"" + sirena.Title + "\" :\n");
    builder.Append(owner)
      .Append(" - sirena owner\n");
    int number = 0;
    foreach (var responsible in responsibles)
    {
      ++number;
      builder.Append(number).Append(". ").Append(responsible).AppendLine();
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

  static public async Task<string> GetUsername(TelegramBot bot, long uid)
  {
    var getChat = new GetChat { ChatId = uid };
    var requestingUserChat = await bot.GetChat(getChat);
    if (requestingUserChat == null)
      return string.Empty;
    return !string.IsNullOrEmpty(requestingUserChat.Username) ? '@' + requestingUserChat.Username :
        (requestingUserChat.FirstName + requestingUserChat.LastName);
  }
}