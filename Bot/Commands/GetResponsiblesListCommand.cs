using Hedgey.Extensions;
using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class GetResponsiblesListCommand : AbstractBotCommmand
{
  public const string NAME = "responsible";
  public const string DESCRIPTION = "Display list of user allowed to call Sirena";
  const string wrongParamMessage = "Please use next syntax to get list of responsible users:\n/responsible {sirena id or number}";
  const string noSirenaMessage = "There is no sirena with id *{0}*";
  const string noSirenaWithNumber = "You don't have sirena with number *{0}*";
  private readonly IMongoCollection<UserRepresentation> users;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly TelegramBot bot;
  private readonly FacadeMongoDBRequests requests;

  public GetResponsiblesListCommand( IMongoDatabase db, FacadeMongoDBRequests requests, TelegramBot bot)
  : base(NAME, DESCRIPTION)
  {
    users = db.GetCollection<UserRepresentation>("users");
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    this.bot = bot;
    this.requests = requests;
  }

  public override async void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string param = context.GetArgsString().GetParameterByNumber(0);
    SirenRepresentation sirena;

    if (int.TryParse(param, out int number))
    {
      var result = await requests.GetSirenaBySerialNumber(uid, number);
      if (result != null)
      {
        sirena = result;
      }
      else
      {
        Program.botProxyRequests.Send(chatId, string.Format(noSirenaWithNumber, number));
        return;
      }
    }
    else if (ObjectId.TryParse(param, out ObjectId id))
    {
      sirena = await requests.GetSirenaById(id);
      if (sirena == null)
      {

        Program.botProxyRequests.Send(chatId, noSirenaMessage);
        return;
      }
    }
    else
    {
      Program.botProxyRequests.Send(chatId, wrongParamMessage);
      return;
    }

    var messageText = await CreateMessageText(sirena);
    Program.botProxyRequests.Send(chatId, messageText);
  }

  private async Task<string[]> GetResponsibleNames(SirenRepresentation sirena)
  {
    string[] names = new string[sirena.Responsible.Length];
    for (int id = 0; id != sirena.Responsible.Length; ++id)
    {
      var chat = await bot.GetChatByUID(sirena.Responsible[id]);
      names[id] = chat?.Username ?? string.Empty;
      names[id] += "|" + sirena.Responsible[id];
    }

    return names;
  }

  private async Task<string> CreateMessageText(SirenRepresentation? sirena)
  {
    if (sirena == null)
    {
      const string message = "Sirena wasn't found";
      return message;
    }

    var chat = await bot.GetChatByUID(sirena.OwnerId);
    string owner = chat?.Username ?? "Ghost";
    owner += "|" + sirena.OwnerId;

    var builder = new StringBuilder("Sirena *\"")
          .Append(sirena.Title)
          .Append("\"* supervisors:\n")
          .Append(owner)
          .Append(" - Owner\n");

    if (sirena.Responsible.Any())
    {
      int number = 0;
      string[] responsibles = await GetResponsibleNames(sirena);

      foreach (var responsible in responsibles)
      {
        ++number;
        builder.Append(number).Append(". ").Append(responsible).AppendLine();
      }
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