namespace Hedgey.Extensions;

static public class OSTools{

  public static string GetEnvironmentVar(string key)
  {
    var value = Environment.GetEnvironmentVariable(key);
    if (string.IsNullOrEmpty(value))
    {
      Environment.FailFast(key + " is not set. Please set environment variable");
    }
    return value;
}}