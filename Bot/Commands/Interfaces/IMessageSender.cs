using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public interface IMessageSender{
  void Send(long chatId, string text, bool silent = true);
  public record struct Result(bool success,Message? Message =null , bool isError = false, string? errorMessage =null);
}
