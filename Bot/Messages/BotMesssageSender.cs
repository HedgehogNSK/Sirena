using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot
{

  public class BotMesssageSender : IMessageSender
  {
    private readonly TelegramBot bot;

    public BotMesssageSender(TelegramBot bot)
    {
      this.bot = bot;
    }

    public async void Send(long chatId, string text, bool silent)
    {

      var sendMessage = new SendMessage
      {
        ChatId = chatId,
        Text = text,
        ProtectContent = false,
        DisableNotification = silent
      };

      try
      {
        RxTelegram.Bot.Interface.BaseTypes.Message message = await bot.SendMessage(sendMessage);
        var result = message;
      }
      catch (ApiException ex)
      {
        var sendException = new Exception($"Exception on sending text to chat: {chatId} reason: "+ex.StatusCode, ex);
        Console.WriteLine(sendException);
      }
    }
  }
}