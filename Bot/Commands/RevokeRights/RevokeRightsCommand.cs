using Hedgey.Localization;
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
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;

  public RevokeRightsCommand(FacadeMongoDBRequests requests, TelegramBot bot
  , ILocalizationProvider localizationProvider
  , IMessageSender messageSender)
  : base(NAME, DESCRIPTION)
  {
    this.bot = bot;
    this.requests = requests;
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
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
      string errorWrongParamters = localizationProvider.Get("command.revoke_rights.incorrect_parameters", info);
      messageSender.Send(chatId, errorWrongParamters);
      return;
    }
    ObjectId sirenaId = default;
    string sirenaIdString = parameters[1];
    if (!int.TryParse(sirenaIdString, out int number)
        && !ObjectId.TryParse(sirenaIdString, out sirenaId))
    {
      string errorWrongSirenaID = localizationProvider.Get("command.revoke_rights.incorrect_sirena_param", info);
      responseText = string.Format(errorWrongSirenaID, sirenaIdString);
      messageSender.Send(chatId, responseText);
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
      string errorWrongUID = localizationProvider.Get("command.revoke_rights.incorrect_uid", info);
      responseText = string.Format(errorWrongUID, userIdString);
      messageSender.Send(chatId, responseText);
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
      string errorNoSirena = localizationProvider.Get("command.revoke_rights.fail", info);
      responseText = string.Format(errorNoSirena, sirenaId);
      messageSender.Send(chatId, responseText);
      return;
    }

    string successMessage = localizationProvider.Get("command.revoke_rights.success", info);
    responseText = string.Format(successMessage, ruid, updatedSiren);
    messageSender.Send(chatId, responseText);
  }

  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<RevokeRightsCommand>(container)
  { }
}