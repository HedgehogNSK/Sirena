using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot
{
  public class MuteUserSignalCommand : AbstractBotCommmand
  {
    const string NAME ="mute";
    const string DESCRIPTION = "Mute calls from certain user for certain *sirena*. Calls of the *sirena* from other users will be active anyway";
    private TelegramBot bot;
    private FacadeMongoDBRequests requests;
    const string errorWrongParamters = "Please input: `/mute {user id to mute} {sirena id}`";
    const string errorWrongSirenaID = "{0} parameter is incorrect. Second parameter has to be serial number or ID of your sirena";
    const string errorWrongUID = "{0} parameter is incorrect. First parameter has to be *UID* of user that is already responsible for sirena. And you won't to revoke they rights.";
    const string errorCantMute = "You can't mute this user for the sirena. Possible causes: you aren't subscibed to this sirena, target user isn't responsible for call of this sirena.";
    const string errorNoSirena = "Couldn't find sirena to mute user";
    const string successMessage = "User {0} has been muted. Now you will not get notifications if this user call the sirena: *{1}*";

    public MuteUserSignalCommand( FacadeMongoDBRequests requests, TelegramBot bot)
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

      var isMutable = await requests.IsPossibleToMute(uid,_UIDtoMute,sirenaId);
      if(!isMutable){
        Program.messageSender.Send(chatId, errorCantMute);
        return;
      }
      var result = await requests.SetUserMute(uid,_UIDtoMute, sirenaId);
      if(result ==null)
      {
        Program.messageSender.Send(chatId, errorNoSirena);
       return;
      }
        responseText = string.Format(successMessage, _UIDtoMute, sirenaId);
        Program.messageSender.Send(chatId, responseText);
    }
  }
}