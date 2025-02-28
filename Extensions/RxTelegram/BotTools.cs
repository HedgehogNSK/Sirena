using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Chats;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Extensions.Telegram;

public static class BotTools
{
  [Obsolete("Use IGetUserInformation.GetNickname instead")]
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

  private static string ComposeName(string Username, string FirstName, string LastName)
    => !string.IsNullOrEmpty(Username) ? '@' + Username : (FirstName + ' ' + LastName);
  public static string GetChatName(this Chat chat)
    => ComposeName(chat.Username, chat.FirstName, chat.LastName);
  public static string GetMemberName(this ChatMember chatMember)
    => GetDisplayName(chatMember.User);
  public static string GetDisplayName(this ChatFullInfo chatInfo)
    => ComposeName(chatInfo.Username, chatInfo.FirstName, chatInfo.LastName);
  public static string GetDisplayName(this User user)
    => ComposeName(user.Username, user.FirstName, user.LastName);
  [Obsolete("Use IGetUserInformation.GetNickname instead")]
  public static async Task<string> GetDisplayName(this TelegramBot bot, long uid)
  {
    var getChatMember = new GetChatMember()
    {
      UserId = uid,
      ChatId = uid
    };
    try
    {
      var chatMember = await bot.GetChatMember(getChatMember);
      if (chatMember == null)
        return "Ghost";
      return chatMember.GetMemberName();
    }
    catch (RxTelegram.Bot.Exceptions.ApiException apiEx)
    {
      if (apiEx.ErrorCode == RxTelegram.Bot.Exceptions.ErrorCode.ChatNotFound)
        Console.WriteLine($"Chat with id:\'{uid}\' not found. Probably it's a ghost or test user");
      else
        Console.WriteLine(apiEx);
      return string.Empty;
    }
    catch (RxTelegram.Bot.Exceptions.RequestValidationException ex)
    {
      Console.WriteLine(ex);
      foreach (var error in ex.ValidationErrors)
        Console.WriteLine(error.GetMessage);
      return string.Empty;
    }
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