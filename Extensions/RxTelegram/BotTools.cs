using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Chats;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Extensions.Telegram;

public static class BotTools
{

  static public async Task<ChatFullInfo?> GetChatByUID(this TelegramBot bot, long uid)
  {
    var getChat = new GetChat { ChatId = uid };
    try
    {
      var chat = await bot.GetChat(getChat);
      return chat;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return default;
    }
  }
  public static string GetUsername(this Chat chat)
    => !string.IsNullOrEmpty(chat.Username) ? '@' + chat.Username :
        (chat.FirstName + chat.LastName);
  public static string GetUsername(this User user)
    => !string.IsNullOrEmpty(user.Username) ? '@' + user.Username :
        (user.FirstName + user.LastName);

  public static async Task<string> GetUsername(TelegramBot bot, long uid)
  {
    var chat = await bot.GetChatByUID(uid);
    return chat?.Username ?? "Ghost";
  }

  public static CopyMessages Clone(this CopyMessages source){
    return new CopyMessages(){
       ChatId = source.ChatId,
        DisableNotification = source.DisableNotification,
        FromChatId = source.FromChatId,
        MessageIds = source.MessageIds,
        MessageThreadId = source.MessageThreadId,
        ProtectContent = source.ProtectContent,
        RemoveCaption = source.RemoveCaption
    };
  }
}