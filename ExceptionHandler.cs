using System.Text;
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
      StringBuilder builder = new StringBuilder($"<{i}> {name}: {current.Message}\n");
      switch (current)
      {
        case ApiException apiException:
          {
            builder.Append($"Description: {apiException.Description}\n{current.StackTrace}");
            Console.WriteLine(builder);
          }
          break;
        case RequestValidationException validationException:
          {
            builder.AppendLine("validateion errors:");
             foreach(var error in validationException.ValidationErrors)
              builder.AppendLine(error.GetMessage);
             Console.WriteLine(builder); 
          }
          break;
        default: Console.WriteLine(builder.AppendLine(current.StackTrace)); break;
      }
      current = current.InnerException;
      ++i;
      builder.Clear();
    }
    while (current != null);
  }
}