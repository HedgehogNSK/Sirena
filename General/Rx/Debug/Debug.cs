
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Rx.Debug;
static public class Extensions
{
  public static IObservable<T> Debug<T>(this IObservable<T> source, string title)
   => source.Materialize()
      .Do(x =>
      {
        StringBuilder builder = new StringBuilder(title).Append(": ");
        switch (x.Kind)
        {
          case System.Reactive.NotificationKind.OnNext: builder.Append($"OnNext->").Append(x.Value); break;
          case System.Reactive.NotificationKind.OnCompleted: builder.Append($"Complete"); break;
          case System.Reactive.NotificationKind.OnError: builder.Append($"Error->").Append(x.Exception); break;
        }
        Console.WriteLine(builder);
      }).Dematerialize();
  public static IObservable<T> Debug<T>(this IObservable<T> source, string title, Func<T,string> toString)
   => source.Materialize()
      .Do(x =>
      { StringBuilder builder = new StringBuilder(title).Append(": ");
        switch (x.Kind)
        {
          case System.Reactive.NotificationKind.OnNext: builder.Append($"OnNext->").Append(toString(x.Value)); break;
          case System.Reactive.NotificationKind.OnCompleted: builder.Append($"Complete"); break;
          case System.Reactive.NotificationKind.OnError: builder.Append($"Error->").Append(x.Exception); break;
        }
        Console.WriteLine(builder);
      }).Dematerialize();
}