using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Blendflake;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class UnmuteUserSignalCommand : AbstractBotCommmand
{
  public const string NAME = "unmute";
  public const string DESCRIPTION = "Unmute previously muted user for certain *Sirena";
  private TelegramBot bot;
  private FacadeMongoDBRequests requests;
  private readonly IMessageSender messageSender;
  private readonly ILocalizationProvider localizationProvider;

  public UnmuteUserSignalCommand(FacadeMongoDBRequests requests
  , TelegramBot bot, IMessageSender messageSender
  , ILocalizationProvider localizationProvider)
: base(NAME, DESCRIPTION)
  {
    this.bot = bot;
    this.requests = requests;
    this.messageSender = messageSender;
    this.localizationProvider = localizationProvider;
  }

  public async override void Execute(IRequestContext context)
  {
    string responseText;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    System.Globalization.CultureInfo cultureInfo = context.GetCultureInfo();
    string[] parameters = context.GetArgsString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length < 2)
    {
      var errorWrongParamters = localizationProvider.Get("command.unmute.error.wrong_parameter", cultureInfo);
      messageSender.Send(chatId, errorWrongParamters);
      return;
    }
    var sirenaIdString = parameters[1];
    var userIdString = parameters[0];
    ulong sirenaId = default;
    if (!int.TryParse(sirenaIdString, out int number)
        && !HashUtilities.TryParse(sirenaIdString, out sirenaId))
    {
      var errorWrongSirenaID = localizationProvider.Get("command.unmute.error.wrong_sirena_id", cultureInfo);
      responseText = string.Format(errorWrongSirenaID, sirenaIdString);
      messageSender.Send(chatId, responseText);
      return;
    }
    ChatFullInfo? chat = null;
    if (long.TryParse(userIdString, out long _UIDtoMute))
    {
      chat = await Extensions.Telegram.BotTools.GetChatByUID(bot, _UIDtoMute);
    }
    if (chat == null)
    {
      var errorWrongUID = localizationProvider.Get("command.unmute.error.wrong_uid", cultureInfo);
      responseText = string.Format(errorWrongUID, userIdString);
      messageSender.Send(chatId, responseText);
      return;
    }
    _UIDtoMute = chat.Id;

    var result = await requests.UnmuteUser(uid, _UIDtoMute, sirenaId);
    if (result == null)
    {
      var errorDidntUnmute = localizationProvider.Get("command.unmute.error.not_found", cultureInfo);
      messageSender.Send(chatId, errorDidntUnmute);
      return;
    }
    var successMessage = localizationProvider.Get("command.unmute.success", cultureInfo);
    responseText = string.Format(successMessage, _UIDtoMute, sirenaId);
    messageSender.Send(chatId, responseText);
  }
}