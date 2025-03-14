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
  public static string BuildString(this IEnumerable<char> chars)
  {
    var builder = new StringBuilder();
    foreach (var c in chars)
      builder.Append(c);
    return builder.ToString();
  }

  public static string AssembleString(this IEnumerable<char> chars)
  => new string(chars.ToArray());
  public static string RemoveCommandName(this string source)
  {
    source = source.TrimStart();
    if (source[0] != '/') return source;
    return source.SkipWords(1).AssembleString();
  }
  public static IEnumerable<char> SkipWords(this IEnumerable<char> input, int n)
  {
    if (n < 0)
      throw new ArgumentException("N has to be greater or equal to 0", "n");
    if (n == 0) return input;
    input = input.SkipWhile(char.IsWhiteSpace);
    for (int id = 0; id != n; ++id)
    {
      input = input.SkipWhile(_c => !char.IsWhiteSpace(_c))
          .SkipWhile(char.IsWhiteSpace);
    }
    return input;
  }
  public static string GetParameterByNumber(this IEnumerable<char> commandString, int number)
    => commandString.SkipWords(number)
        .TakeWhile(_char => !char.IsWhiteSpace(_char))
        .AssembleString();

  public static bool ExtractCommandAndArgs(string source, out string command, out string argString)
  {
    argString = string.Empty;
    command = string.Empty;
    if (string.IsNullOrEmpty(source)) return false;

    source = source.TrimStart();
    if (source[0] != '/')
    {
      argString = source;
      return false;
    }
    command = source.Skip(1).GetParameterByNumber(0);
    argString = source.SkipWords(1).AssembleString();
    return true;
  }
  private static readonly char[] markdownV2charsToEscape = ['_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!'];
  private static readonly char[] markdowncharsToEscape = ['_', '*', '[', '`'];
  public static string EscapeChars(this string input, char[] charsToEscape)
  {
    if (string.IsNullOrEmpty(input))
      return input;

    var result = new StringBuilder(input.Length * 2);

    foreach (char c in input)
    {
      if (Array.Exists(charsToEscape, ch => ch == c))
        result.Append('\\');

      result.Append(c);
    }
    return result.ToString();
  }
  public static string EscapeMarkdownV2Chars(this string input)  
  => EscapeChars(input, markdownV2charsToEscape);
  
  public static string EscapeMarkdownChars(this string input)  
  => EscapeChars(input, markdowncharsToEscape);
}