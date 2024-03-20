namespace Hedgey.Extensions;

static public class OSTools{

  public static string GetEnvironmentVar(string key)
  {
    var value = Environment.GetEnvironmentVariable(key);
    if (string.IsNullOrEmpty(value))
    {
      Console.WriteLine(key+" is not set");
      Environment.Exit(0);
    }
    return value;
}}