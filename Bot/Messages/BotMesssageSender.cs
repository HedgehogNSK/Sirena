using RxTelegram.Bot;
using RxTelegram.Bot.Exceptions;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
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

    public void Send(ChatId chatId, string text,IReplyMarkup? markup, bool silent)
    {

      var sendMessage = new SendMessage
      {
        ChatId = chatId,
        Text = text,
        ReplyMarkup =  markup,
        ProtectContent = false,
        DisableNotification = silent
      };
       Send(sendMessage);
    }

    public async void Send(SendMessage sendMessage)
    {

      try
      {
        RxTelegram.Bot.Interface.BaseTypes.Message message = await bot.SendMessage(sendMessage);
        var result = message;
      }
      catch (ApiException ex)
      {
        var sendException = new Exception($"Exception on sending text to chat: {sendMessage.ChatId} reason: {ex.StatusCode}\n{ex.Description}", ex);
        Console.WriteLine(sendException);
      }
    }
  }
}