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
    =>  GetNickname(uid, CultureInfo.InvariantCulture);
  public IObservable<string> GetNickname(long userID, CultureInfo info)
    => GetNickname(userID, userID, info);
  public IObservable<string> GetNickname(long uid, long cid, CultureInfo info)
  {
    var getChatMember = new GetChatMember()
    {
      UserId = uid,
      ChatId = cid
    };
    return Observable.FromAsync(() => bot.GetChatMember(getChatMember))
    .Select(_chatMember => _chatMember.GetMemberName())
    .Catch((Exception ex) =>
    {
      switch (ex)
      {
        case RxTelegram.Bot.Exceptions.ApiException apiEx:
          {
            if (apiEx.ErrorCode == RxTelegram.Bot.Exceptions.ErrorCode.ChatNotFound)
              Console.WriteLine($"Chat with id:\'{uid}\' not found. Probably it's a ghost or test user");
            else
              Console.WriteLine($"{apiEx.GetType().FullName}: {apiEx.Message}\nDescription: {apiEx.Description}\n{apiEx.StackTrace}");
          } break;
        case RxTelegram.Bot.Exceptions.RequestValidationException validationEx:
          {
            Console.WriteLine(validationEx);
            foreach (var error in validationEx.ValidationErrors)
              Console.WriteLine(error.GetMessage);
          } break;
        default:
          {
            var errorMessage = $"Exception on attempt to get chat member with user id: {getChatMember.UserId} in chat: {getChatMember.ChatId}";
            ex = new Exception(errorMessage, ex);
            Console.WriteLine(ex);
          } break;
      }
      return Observable.Return( localizationProvider.Get("miscellaneous.user_ghost", info));
    });
  }
}