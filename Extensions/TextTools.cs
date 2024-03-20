using System.Text;

namespace Hedgey.Extensions;

static public class TextTools
{
  /// <href>https://stackoverflow.com/a/11654221/9043688<href>
  /// ToArrayString:        00:00:03.1695463
  /// Concat:               00:00:07.2518054
  /// StringBuilderChars:   00:00:03.1335455
  /// StringBuilderStrings: 00:00:06.4618266
  /// <summary>
  /// Converts enumerable of chars to string. The fastest that I've found
  /// </summary>
  /// <param name="chars"></param>
  /// <returns></returns>
  public static string ConvertToString(this IEnumerable<char> chars)
  {
    var builder = new StringBuilder();
    foreach (var c in chars)
      builder.Append(c);
    return builder.ToString();
  }
  
  public static string SkipFirstNWords(this string input, int n)
    {
        input = input.TrimStart();
        int index = 0;
        for (int i = 0; i < n; i++)
        {
            index = input.IndexOf(' ', index);
            if (index == -1) return string.Empty;
            ++index;
        }
        return input.Substring(index);
    }
  public static string GetParameterByNumber(string text, int number)
  => text.SkipWhile(_char => _char != ' ')
          .Skip(number)
          .TakeWhile(_char => _char != ' ')
          .ConvertToString();
}