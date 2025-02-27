using Hedgey.Extensions;
using RxTelegram.Bot.Exceptions;

namespace Hedgey.Sirena;

static public class ExceptionHandler
{
  static public void OnError(Exception exception)
  {
    var time = Shortucts.CurrentTimeLabel();
    var current = exception;
    Console.WriteLine(time + "EXCEPTION!!!");
    int i = 0;
    do
    {
      string? name = current.GetType().FullName;
      switch (current)
      {
        case ApiException apiException:
          {
            string message = $"<{i}> {name}: {current.Message}\nDescription: {apiException.Description}\n{current.StackTrace}";
            Console.WriteLine(message);
          }
          break;
        default: Console.WriteLine($"<{i}> {name}: {current.Message}\n{current.StackTrace}"); break;
      }
      current = current.InnerException;
      ++i;
    }
    while (current != null);
  }
}