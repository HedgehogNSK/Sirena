using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Chats;
using System.Globalization;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot.Operations;

public class GetUserInformation : IGetUserInformation
{
  private readonly TelegramBot bot;
  private readonly ILocalizationProvider localizationProvider;

  public GetUserInformation(TelegramBot bot, ILocalizationProvider localizationProvider)
  {
    this.bot = bot;
    this.localizationProvider = localizationProvider;
  }
  public IObservable<string> GetNickname(long uid)
    => GetNickname(uid, CultureInfo.InvariantCulture);
  public IObservable<string> GetNickname(long userID, CultureInfo info)
    => GetNickname(userID, userID, info);
  /// <summary>
  /// Get user name by id from certain chat
  /// DO NOT user this function for private chats
  /// USE this function only for group chats
  /// For private chats user id must be equal to chat id
  /// </summary>
  /// <param name="uid"></param>
  /// <param name="cid"></param>
  /// <param name="info"></param>
  /// <returns></returns>
  public IObservable<string> GetNickname(long uid, long cid, CultureInfo info)
  {
    var getChatMember = new GetChatMember()
    {
      UserId = uid,
      ChatId = cid
    };
    return Observable.FromAsync(() => bot.GetChatMember(getChatMember))
    .Select(_chatMember =>
    {
      return _chatMember.GetMemberName();
    })
    .Catch((Exception ex) =>
    {
      Console.WriteLine($"Exception happen on attempt to get user nickname of user:{uid} in chat:{cid}");
      switch (ex)
      {
        case RxTelegram.Bot.Exceptions.ApiException apiEx:
          {
            Console.WriteLine($"{apiEx.GetType().FullName}: {apiEx.Message}]\nError Code: {apiEx.ErrorCode}\nDescription: {apiEx.Description}\n{apiEx.StackTrace}");
          }
          break;
        case RxTelegram.Bot.Exceptions.RequestValidationException validationEx:
          {
            Console.WriteLine(validationEx);
            foreach (var error in validationEx.ValidationErrors)
              Console.WriteLine(error.GetMessage);
          }
          break;
        default:
          {
            Console.WriteLine(ex);
          }
          break;
      }
      return Observable.Return(localizationProvider.Get("miscellaneous.user_ghost", info));
    });
  }
}