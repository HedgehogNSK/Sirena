using System.Text;
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
    catch (RxTelegram.Bot.Exceptions.ApiException apiEx)
    {
      if (apiEx.ErrorCode == RxTelegram.Bot.Exceptions.ErrorCode.ChatNotFound)
        Console.WriteLine($"Chat with id:\'{uid}\' not found. Probably it's a ghost or test user");
      else
        Console.WriteLine(apiEx);
      return default;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return default;
    }
  }

  private static string GetDisplayName(string Username, string FirstName, string LastName)
    => !string.IsNullOrEmpty(Username) ? '@' + Username : (FirstName + ' ' + LastName);
  public static string GetDisplayName(this Chat chat)
    => GetDisplayName(chat.Username, chat.FirstName, chat.LastName);
  public static string GetDisplayName(this ChatFullInfo chatInfo)
    => GetDisplayName(chatInfo.Username, chatInfo.FirstName, chatInfo.LastName);

  public static string GetDisplayName(this User user)
    => GetDisplayName(user.Username, user.FirstName, user.LastName);

  public static async Task<string> GetDisplayName(this TelegramBot bot, long uid)
  {
    var chat = await bot.GetChatByUID(uid);
    if (chat == null)
      return "Ghost";
    return chat.GetDisplayName();
  }

  public static CopyMessages Clone(this CopyMessages source)
    => new CopyMessages()
    {
      ChatId = source.ChatId,
      DisableNotification = source.DisableNotification,
      FromChatId = source.FromChatId,
      MessageIds = source.MessageIds,
      MessageThreadId = source.MessageThreadId,
      ProtectContent = source.ProtectContent,
      RemoveCaption = source.RemoveCaption
    };
  public static async Task DisplayWebhookInfo(this TelegramBot bot)
  {
    var whinfo = await bot.GetWebhookInfo();
    var allowedUpdates = whinfo.AllowedUpdates?.Aggregate(string.Empty, (a, b) => string.Concat(a, ' ', b)) ?? null;
    StringBuilder builder = new StringBuilder("___WEBHOOK INFO___\n");
    builder.Append("Listener IP: ").AppendLine(whinfo.IpAddress)
    .Append("Has certificate: ").Append(whinfo.HasCustomCertificate).AppendLine()
    .Append("Allowed updates:").AppendLine(allowedUpdates)
    .Append("Pending updates count: ").Append(whinfo.PendingUpdateCount).AppendLine();

    if (whinfo.LastErrorDate != null)
      builder.Append("Error message: [").Append(whinfo.LastErrorDate).Append(']').AppendLine(whinfo.LastErrorMessage);

    builder.AppendLine("------------------");

    Console.WriteLine(builder.ToString());
  }
}