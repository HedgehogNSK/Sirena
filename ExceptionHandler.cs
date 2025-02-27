using Hedgey.Extensions;
using RxTelegram.Bot.Exceptions;

namespace Hedgey.Sirena;

static public class ExceptionHandler
{
  static public void OnError(Exception exception){
    var time = Shortucts.CurrentTimeLabel();
    var current = exception;
    Console.WriteLine(time + "EXCEPTION!!!");
    int i=0;
    do
    {
      switch (current)
      {
        case ApiException apiException:
          {
            string message = $"<{i}> {current.GetType().FullName}: {apiException.Message}\nDescription: {apiException.Description}\n{apiException.StackTrace}";
            Console.WriteLine(message);
          }
          break;
        default: Console.WriteLine($"<{i}> {current.GetType().FullName}: {current.Message}\n{current.StackTrace}"); break;
      }
      current = current.InnerException;
      ++i;
    }
    while (current != null);
  }
}