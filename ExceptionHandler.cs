using Hedgey.Extensions;
using RxTelegram.Bot.Exceptions;

namespace Hedgey.Sirena;

static public class ExceptionHandler
{
  static public void OnError(Exception exception){
    var time = Shortucts.CurrentTimeLabel();
    var ex = exception;
    do
    {
      switch (ex)
      {
        case ApiException apiException:
          {
            string message = time + apiException.Message + ": " + apiException.Description;
            Console.WriteLine(message);
          }
          break;
        default: Console.WriteLine(time + exception.Message); break;
      }
      ex = ex.InnerException;
    }
    while (ex != null);
  }
}