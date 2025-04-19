using System.Reactive.Linq;
using Hedgey.Blendflake;
using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.MongoDB;
using Hedgey.Telegram.Bot;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class UnmuteUserSignalCommand(FacadeMongoDBRequests requests
  , TelegramBot bot, IMessageSender messageSender
  , ILocalizationProvider localizationProvider
  , IMessageEditor messageEditor
  , IGetUserInformation userInformation)
  : AbstractBotCommmand(NAME, DESCRIPTION)
{
  public const string NAME = "unmute";
  public const string DESCRIPTION = "Unmute previously muted user for certain *Sirena";
  private readonly TelegramBot bot = bot;
  private readonly FacadeMongoDBRequests requests = requests;
  private readonly IMessageSender messageSender = messageSender;
  private readonly ILocalizationProvider localizationProvider = localizationProvider;
  private readonly IMessageEditor messageEditor = messageEditor;

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
    if (!int.TryParse(sirenaIdString, out _)
        && !HashUtilities.TryParse(sirenaIdString, out sirenaId))
    {
      var errorWrongSirenaID = localizationProvider.Get("command.unmute.error.wrong_sirena_id", cultureInfo);
      responseText = string.Format(errorWrongSirenaID, sirenaIdString);
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
      var errorWrongUID = localizationProvider.Get("command.unmute.error.wrong_uid", cultureInfo);
      responseText = string.Format(errorWrongUID, userIdString);
      messageSender.Send(chatId, responseText);
      return;
    }
    uidToMute = chat.Id;
    
    var sirena = await requests.UnmuteUser(uid, uidToMute, sirenaId);
    if (sirena == null)
    {
      var errorDidntUnmute = localizationProvider.Get("command.unmute.error.not_found", cultureInfo);
      messageSender.Send(chatId, errorDidntUnmute);
      return;
    }
    
    var successMessage = localizationProvider.Get("command.unmute.success", cultureInfo);
    var nickname = chat.GetDisplayName();
    responseText = string.Format(successMessage, nickname, uidToMute, sirena.ToString());
    messageSender.Send(chatId, responseText);

    if (context.GetMessage().From.IsBot)
    {
      var option = new SwitchButtonCommandReplyMarkupBuilder.Option(MuteUserSignalCommand.NAME, MarkupShortcuts.muteTitle);
      var editReplyMarkup = new SwitchButtonCommandReplyMarkupBuilder(localizationProvider, option, context);
      messageEditor.Edit(editReplyMarkup).Subscribe();
    }
  }
}