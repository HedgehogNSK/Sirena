using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public interface IMessageSender{
  void Send(long chatId, string text, bool silent = true);
}