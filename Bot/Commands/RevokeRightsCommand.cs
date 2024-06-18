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
  public RevokeRightsCommand(FacadeMongoDBRequests requests, TelegramBot bot)
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
    var info = context.GetCultureInfo();
    string[] parameters = context.GetArgsString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length < 3)
    {
      string errorWrongParamters = Program.LocalizationProvider.Get("command.revoke_rights.incorrect_parameters", info);
      Program.botProxyRequests.Send(chatId, errorWrongParamters);
      return;
    }
    ObjectId sirenaId = default;
    string sirenaIdString = parameters[1];
    if (!int.TryParse(sirenaIdString, out int number)
        && !ObjectId.TryParse(sirenaIdString, out sirenaId))
    {
      string errorWrongSirenaID = Program.LocalizationProvider.Get("command.revoke_rights.incorrect_sirena_param", info);
      responseText = string.Format(errorWrongSirenaID, sirenaIdString);
      Program.botProxyRequests.Send(chatId, responseText);
      return;
    }
    ChatFullInfo? chat = null;
    string userIdString = parameters[2];
    if (long.TryParse(userIdString, out long ruid))
    {
      chat = await Extensions.Telegram.BotTools.GetChatByUID(bot, ruid);
    }
    if (chat == null)
    {
      string errorWrongUID = Program.LocalizationProvider.Get("command.revoke_rights.incorrect_uid", info);
      responseText = string.Format(errorWrongUID, userIdString);
      Program.botProxyRequests.Send(chatId, responseText);
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
      string errorNoSirena = Program.LocalizationProvider.Get("command.revoke_rights.fail", info);
      responseText = string.Format(errorNoSirena, sirenaId);
      Program.botProxyRequests.Send(chatId, responseText);
      return;
    }

    string successMessage = Program.LocalizationProvider.Get("command.revoke_rights.success", info);
    responseText = string.Format(successMessage, ruid, updatedSiren);
    Program.botProxyRequests.Send(chatId, responseText);
  }
}