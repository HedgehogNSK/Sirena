using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class RevokeRightsCommand : AbstractBotCommmand
{
  public const string NAME = "revoke";
  public const string DESCRIPTION = "Allows to request rights to call certain sirena of another user.";
  private readonly FacadeMongoDBRequests requests;
  private readonly TelegramBot bot;
  const string errorWrongParamters = "Please input: /revoke {sirena number or id} {user id}";
  const string errorWrongSirenaID = "{0} parameter is incorrect. First parameter has to be serial number or ID of your sirena";
  const string errorWrongUID = "{0} parameter is incorrect. Second parameter has to be *UID* of user that is already responsible for sirena. And you won't to revoke they rights.";
  const string errorNoSirena = "You couldn't revoke rights of this user. Possible reasons: user doesn't have rights or you don't own this sirena ";
  const string successMessage = "The user: {0} has been deprived of his rights to call the sirena:\n {1}";
  public RevokeRightsCommand( FacadeMongoDBRequests requests, TelegramBot bot)
  : base(NAME, DESCRIPTION)
  {
    this.bot = bot;
    this.requests = requests;
  }

  public async override void Execute(IRequestContext context)
  {
    string responseText;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string[] parameters = context.GetArgsString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length < 3)
    {
      Program.messageSender.Send(chatId, errorWrongParamters);
      return;
    }
    ObjectId sirenaId = default;
    string sirenaIdString = parameters[1];
    if (!int.TryParse(sirenaIdString, out int number)
        && !ObjectId.TryParse(sirenaIdString, out sirenaId))
    {
      responseText = string.Format(errorWrongSirenaID, sirenaIdString);
      Program.messageSender.Send(chatId, responseText);
      return;
    }
    Chat? chat = null;
    string userIdString = parameters[2];
    if (long.TryParse(userIdString, out long ruid))
    {
      chat = await Extensions.Telegram.BotTools.GetChatByUID(bot, ruid);
    }
    if (chat == null)
    {
      responseText = string.Format(errorWrongUID, userIdString);
      Program.messageSender.Send(chatId, responseText);
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
      Program.messageSender.Send(chatId, responseText);
      return;
    }

    responseText = string.Format(successMessage, ruid, updatedSiren);
    Program.messageSender.Send(chatId, responseText);
  }
}