using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Chats;

namespace Hedgey.Extensions.Telegram;

public static class BotTools
{

  static public async Task<Chat> GetChatByUID(TelegramBot bot, long uid)
  {
    var getChat = new GetChat { ChatId = uid };
    try{
          var chat = await bot.GetChat(getChat);
    return chat;
    }
    catch(Exception ex)
    {
      Console.WriteLine(ex);
       return null;
    }
  }
  public static string GetUsername(this Chat chat)
    =>  !string.IsNullOrEmpty(chat.Username) ? '@' + chat.Username :
        (chat.FirstName + chat.LastName);
  public static string GetUsername(this User user)
    =>  !string.IsNullOrEmpty(user.Username) ? '@' + user.Username :
        (user.FirstName + user.LastName);
}