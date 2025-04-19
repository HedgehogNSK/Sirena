using Hedgey.Blendflake;
using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.MongoDB;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class MuteUserSignalCommand(FacadeMongoDBRequests requests, TelegramBot bot
  , ILocalizationProvider localizationProvider, IMessageSender messageSender
  , IMessageEditor messageEditor)
  : AbstractBotCommmand(NAME, DESCRIPTION)
{
  public const string NAME = "mute";
  public const string DESCRIPTION = "Mute calls from certain user for certain *sirena*. Calls of the *sirena* from other users will be active anyway";
  private readonly ILocalizationProvider localizationProvider = localizationProvider;
  private readonly IMessageSender messageSender = messageSender;
  private readonly IMessageEditor messageEditor = messageEditor;
  private readonly TelegramBot bot = bot;
  private readonly FacadeMongoDBRequests requests = requests;

  public async override void Execute(IRequestContext context)
  {
    string responseText;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    string[] parameters = context.GetArgsString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length < 2)
    {
      string errorWrongParamters = localizationProvider.Get("command.mute_user.incorrect_parameters", info);
      messageSender.Send(chatId, errorWrongParamters);
      return;
    }
    var sirenaIdString = parameters[1];
    var userIdString = parameters[0];
    ulong sirenaId = default;
    if (!int.TryParse(sirenaIdString, out _)
        && !HashUtilities.TryParse(sirenaIdString, out sirenaId))
    {
      string errorWrongSirenaID = localizationProvider.Get("command.mute_user.incorrect_id", info);
      responseText = string.Format(sirenaIdString, errorWrongSirenaID);
      messageSender.Send(chatId, responseText);
      return;
    }
    ChatFullInfo? chat = null;
    if (long.TryParse(userIdString, out long uidToMute))
    {
      chat = await BotTools.GetChatByUID(bot, uidToMute);
    }
    if (chat == null)
    {
      string errorWrongUID = localizationProvider.Get("command.mute_user.incorrect_uid", info);
      responseText = string.Format(errorWrongUID, userIdString);
      messageSender.Send(chatId, responseText);
      return;
    }
    uidToMute = chat.Id;

    var isMutable = await requests.IsPossibleToMute(uid, uidToMute, sirenaId);
    if (!isMutable)
    {
      string errorCantMute = localizationProvider.Get("command.mute_user.impossbile_mute", info);
      messageSender.Send(chatId, errorCantMute);
      return;
    }
    var sirena = await requests.SetUserMute(uid, uidToMute, sirenaId);
    if (sirena == null)
    {
      string errorNoSirena = localizationProvider.Get("command.mute_user.no_sirena", info);
      messageSender.Send(chatId, errorNoSirena);
      return;
    }
    string successMessage = localizationProvider.Get("command.mute_user.success", info);
    var nickname = chat.GetDisplayName();
    responseText = string.Format(successMessage, nickname, uidToMute, sirena.ToString());
    messageSender.Send(chatId, responseText);

    if (context.GetMessage().From.IsBot)
    {
      var option = new SwitchButtonCommandReplyMarkupBuilder.Option(UnmuteUserSignalCommand.NAME, MarkupShortcuts.unmuteTitle);
      var editReplyMarkup = new SwitchButtonCommandReplyMarkupBuilder(localizationProvider, option, context);
      messageEditor.Edit(editReplyMarkup).Subscribe();
    }
  }
}