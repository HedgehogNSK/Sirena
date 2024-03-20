using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class DelegateRightsCommand : BotCustomCommmand
{
  private readonly FacadeMongoDBRequests requests;
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  private readonly TelegramBot bot;
  const string errorWrongParamters= "Please input: /delegate {siren number or id} {user id}";
  const string errorWrongSirenaID= "{0} parameter is incorrect. First parameter has to be serial number or ID of your sirena";
  const string errorWrongUID= "{0} parameter is incorrect.Second parameter has to be *UID* of user that will have right to call the sirena.";
  const string errorNoSirena= "You don't have a sirena1 with id: {0}" ;
  const string successMessage ="User {0} has been set as a responsible for sirena: {1}";
  public DelegateRightsCommand(string name, string description, IMongoDatabase db, FacadeMongoDBRequests requests, TelegramBot bot)
  : base(name, description)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
    this.bot = bot;
    this.requests = requests;
  }

  public async override void Execute(Message message)
  {
    string responseText;
    long uid = message.From.Id;
    string[] parameters = message.Text.Split(' ',3, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length < 3)
    {
      Program.messageSender.Send(message.Chat.Id, errorWrongParamters);
      return;
    }
    ObjectId sirenaId = default;
    if (!int.TryParse(parameters[1], out int number)
        && !ObjectId.TryParse(parameters[1], out sirenaId))
    {
       responseText = string.Format(errorWrongSirenaID,parameters[1]);
      Program.messageSender.Send(message.Chat.Id, responseText);
      return;
    }
    if (!long.TryParse(parameters[2], out long duid))
    {
      responseText = string.Format(errorWrongUID,parameters[2]);
      Program.messageSender.Send(message.Chat.Id, responseText);
      return;
    }
    if (sirenaId == default)
    {
      //Get id of siren
      var sirena = await requests.GetSirenaBySerialNumber(uid, number);
      if (sirena == null)
        return;

      sirenaId = sirena.Id;
    }

    //Set responsible
    SirenRepresentation updatedSiren = await requests.SetUserResponsible(uid, sirenaId, duid);

    if (updatedSiren == null)
    {
      responseText = string.Format(errorNoSirena,sirenaId);
      Program.messageSender.Send(message.Chat.Id, responseText);
      return;
    }
    responseText = string.Format(successMessage, duid, updatedSiren);
    Program.messageSender.Send(message.Chat.Id, responseText);
  }
}