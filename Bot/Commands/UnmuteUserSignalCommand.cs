using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot
{
  public class UnmuteUserSignalCommand : AbstractBotCommmand
  {
    const string NAME = "unmute";
    const string DESCRIPTION = "Unmute previously muted user for certain siren";
    private TelegramBot bot;
    private FacadeMongoDBRequests requests;
    const string errorWrongParamters = "Please input: /unmute {user id to mute} {sirena id}";
    const string errorWrongSirenaID = "{0} parameter is incorrect. Second parameter has to be serial number or *ID* of your sirena";
    const string errorWrongUID = "{0} parameter is incorrect. First parameter has to be *UID* of user that is already responsible for sirena. And you won't to revoke they rights.";
    const string errorDidntUnmute = "Couldn't find sirena to mute user";
    const string successMessage = "User {0} has been unmuted. You will be notified if this user will call the sirena: *{1}*";

    public UnmuteUserSignalCommand( FacadeMongoDBRequests requests, TelegramBot bot)
  : base(NAME, DESCRIPTION)
    {
      this.bot = bot;
      this.requests = requests;
    }

    public async override void Execute(ICommandContext context)
    {
      string responseText;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
      string[] parameters = context.GetArgsString().Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
      if (parameters.Length < 3)
      {
        Program.messageSender.Send(chatId, errorWrongParamters);
        return;
      }
      var sirenaIdString = parameters[2];
      var userIdString = parameters[1];
      ObjectId sirenaId = default;
      if (!int.TryParse(sirenaIdString, out int number)
          && !ObjectId.TryParse(sirenaIdString, out sirenaId))
      {
        responseText = string.Format(sirenaIdString, errorWrongSirenaID);
        Program.messageSender.Send(chatId, responseText);
        return;
      }
      Chat? chat = null;
      if (long.TryParse(userIdString, out long _UIDtoMute))
      {
        chat = await Extensions.Telegram.BotTools.GetChatByUID(bot, _UIDtoMute);
      }
      if (chat == null)
      {
        responseText = string.Format(errorWrongUID, userIdString);
        Program.messageSender.Send(chatId, responseText);
        return;
      }
      _UIDtoMute = chat.Id;

      var result = await requests.UnmuteUser(uid,_UIDtoMute, sirenaId);
      if(result ==null)
      {
        Program.messageSender.Send(chatId, errorDidntUnmute);
       return;
      }
        responseText = string.Format(successMessage, _UIDtoMute, sirenaId);
        Program.messageSender.Send(chatId, responseText);
    }
  }
}