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

  private static string GetDisplayName(string Username, string FirstName, string LastName)
  {
    return !string.IsNullOrEmpty(Username) ? '@' + Username :
          (FirstName + ' ' + LastName);
  }
  public static string GetDisplayName(this Chat chat)
  => GetDisplayName(chat.Username, chat.FirstName, chat.LastName);
  public static string GetDisplayName(this ChatFullInfo chatInfo)
  => GetDisplayName(chatInfo.Username, chatInfo.FirstName, chatInfo.LastName);

  public static string GetDisplayName(this User user)
  => GetDisplayName(user.Username, user.FirstName, user.LastName);

  public static async Task<string> GetDisplayName(this TelegramBot bot, long uid)
  {
    var chat = await bot.GetChatByUID(uid);
    if(chat==null)
      return "Ghost";
    return chat.GetDisplayName();
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