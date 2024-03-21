using RxTelegram.Bot;
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
        //return new IMessageSender.Result(true,result);
      }
      catch (Exception ex)
      {

        var sendException = new Exception($"Exception on sending text to chat: {chatId}", ex);
        Console.WriteLine(sendException);
        //return new IMessageSender.Result(false, isError: true, errorMessage: sendException.Message );
      }
    }
  }
}