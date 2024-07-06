using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class MuteUserSignalCommand : AbstractBotCommmand
{
  public const string NAME = "mute";
  public const string DESCRIPTION = "Mute calls from certain user for certain *sirena*. Calls of the *sirena* from other users will be active anyway";
  private readonly ILocalizationProvider localizationProvider;
  private TelegramBot bot;
  private FacadeMongoDBRequests requests;

  public MuteUserSignalCommand(FacadeMongoDBRequests requests, TelegramBot bot, ILocalizationProvider localizationProvider)
: base(NAME, DESCRIPTION)
  {
    this.bot = bot;
    this.requests = requests;
    this.localizationProvider = localizationProvider;
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
      string errorWrongParamters = localizationProvider.Get("command.mute_user.incorrect_parameters", info);
      Program.botProxyRequests.Send(chatId, errorWrongParamters);
      return;
    }
    var sirenaIdString = parameters[2];
    var userIdString = parameters[1];
    ObjectId sirenaId = default;
    if (!int.TryParse(sirenaIdString, out int number)
        && !ObjectId.TryParse(sirenaIdString, out sirenaId))
    {
      string errorWrongSirenaID = localizationProvider.Get("command.mute_user.incorrect_id", info);
      responseText = string.Format(sirenaIdString, errorWrongSirenaID);
      Program.botProxyRequests.Send(chatId, responseText);
      return;
    }
    ChatFullInfo? chat = null;
    if (long.TryParse(userIdString, out long _UIDtoMute))
    {
      chat = await Extensions.Telegram.BotTools.GetChatByUID(bot, _UIDtoMute);
    }
    if (chat == null)
    {
      string errorWrongUID = localizationProvider.Get("command.mute_user.incorrect_uid", info);
      responseText = string.Format(errorWrongUID, userIdString);
      Program.botProxyRequests.Send(chatId, responseText);
      return;
    }
    _UIDtoMute = chat.Id;

    var isMutable = await requests.IsPossibleToMute(uid, _UIDtoMute, sirenaId);
    if (!isMutable)
    {
      string errorCantMute = localizationProvider.Get("command.mute_user.impossbile_mute", info);
      Program.botProxyRequests.Send(chatId, errorCantMute);
      return;
    }
    var result = await requests.SetUserMute(uid, _UIDtoMute, sirenaId);
    if (result == null)
    {
      string errorNoSirena = localizationProvider.Get("command.mute_user.no_sirena", info);
      Program.botProxyRequests.Send(chatId, errorNoSirena);
      return;
    }
    string successMessage = localizationProvider.Get("command.mute_user.success", info);
    responseText = string.Format(successMessage, _UIDtoMute, sirenaId);
    Program.botProxyRequests.Send(chatId, responseText);
  }
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<MuteUserSignalCommand>(container)
  { }
}