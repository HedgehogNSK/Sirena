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

  public GetResponsiblesListCommand(IMongoDatabase db, FacadeMongoDBRequests requests, TelegramBot bot)
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
    var info = context.GetCultureInfo();
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
        string noSirenaWithNumber = Program.LocalizationProvider.Get("command.get_responsibles.no_sirena_number", info);
        Program.botProxyRequests.Send(chatId, string.Format(noSirenaWithNumber, number));
        return;
      }
    }
    else if (ObjectId.TryParse(param, out ObjectId id))
    {
      sirena = await requests.GetSirenaById(id);
      if (sirena == null)
      {

        string noSirenaMessage = Program.LocalizationProvider.Get("command.get_responsibles.no_sirena_id", info);
        Program.botProxyRequests.Send(chatId, noSirenaMessage);
        return;
      }
    }
    else
    {
      string wrongParamMessage = Program.LocalizationProvider.Get("command.get_responsibles.incorrect_parameters", info);
      Program.botProxyRequests.Send(chatId, wrongParamMessage);
      return;
    }

    var messageText = await CreateMessageText(sirena, info);
    Program.botProxyRequests.Send(chatId, messageText);
  }

  private async Task<string[]> GetResponsibleNames(SirenRepresentation sirena)
  {
    string[] names = new string[sirena.Responsible.Length];
    for (int id = 0; id != sirena.Responsible.Length; ++id)
    {
      var uid = sirena.Responsible[id];
      string nick = await bot.GetDisplayName(uid);
      names[id] = $"{nick}|{uid}";
    }

    return names;
  }

  private async Task<string> CreateMessageText(SirenRepresentation? sirena, System.Globalization.CultureInfo info)
  {
    if (sirena == null)
      return Program.LocalizationProvider.Get("command.get_responsibles.no_sirena", info);

    string owner = await bot.GetDisplayName(sirena.OwnerId);
    owner += "|" + sirena.OwnerId;
    string template = Program.LocalizationProvider.Get("command.get_responsibles.template", info);
    var builder = new StringBuilder().AppendFormat(template, sirena.Title, owner);

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
  
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<GetResponsiblesListCommand>(container)
  { }
}