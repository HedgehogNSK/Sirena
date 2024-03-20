using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class RevokeRightsCommand : BotCustomCommmand
{
  private readonly FacadeMongoDBRequests requests;
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  private readonly TelegramBot bot;
  const string errorWrongParamters = "Please input: /revoke {sirena number or id} {user id}";
  const string errorWrongSirenaID = "{0} parameter is incorrect. First parameter has to be serial number or ID of your sirena";
  const string errorWrongUID = "{0} parameter is incorrect. Second parameter has to be *UID* of user that is already responsible for sirena. And you won't to revoke they rights.";
  const string errorNoSirena = "You couldn't revoke rights of this user. Possible reasons: user doesn't have rights or you don't own this sirena ";
  const string successMessage = "The user: {0} has been deprived of his rights to call the sirena:\n {1}";
  public RevokeRightsCommand(string name, string description, IMongoDatabase db, FacadeMongoDBRequests requests, TelegramBot bot)
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
      responseText = parameters[1] + errorWrongSirenaID;
      Program.messageSender.Send(message.Chat.Id, responseText);
      return;
    }
    Chat? chat = null;
    if (long.TryParse(parameters[2], out long ruid))
    {
      chat = await Extensions.Telegram.BotTools.GetChatByUID(bot, ruid);
    }
    if (chat == null)
    {
      responseText = string.Format(errorWrongUID, parameters[2]);
      Program.messageSender.Send(message.Chat.Id, responseText);
      return;
    }
    ruid = chat.Id;

    if (sirenaId == default)
    {
      //Get id of siren
      var sirena = await requests.GetSirenaBySerialNumber(uid, number);
      if (sirena == null)
        return;

      sirenaId = sirena.Id;
    }

    //Revoke rights
    SirenRepresentation updatedSiren = await requests.RevokeUserRights(uid, sirenaId, ruid);

    if (updatedSiren == null)
    {
      responseText = errorNoSirena + sirenaId;
      Program.messageSender.Send(message.Chat.Id, responseText);
      return;
    }

    responseText = string.Format(successMessage, ruid, updatedSiren);
    Program.messageSender.Send(message.Chat.Id, responseText);
  }
}