namespace Hedgey.Extensions;

static public class Shortucts{
  static public string ToDefaultFormat(this DateTimeOffset date){
    const string dateFormat = "HH:mm:ss dd.MM.yyyy";
    return date.ToString(dateFormat);
  }
  static public string CurrentTimeLabel()
  => $"[{DateTimeOffset.UtcNow.ToDefaultFormat()}] ";
}
