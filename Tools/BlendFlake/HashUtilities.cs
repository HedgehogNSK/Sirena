using Hedgey.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace Hedgey.Tools.BlendedFlake;

static public class HashUtilities
{

  static Regex regex = new Regex("-*=");
  /// <summary>
  /// Remove last consequent '-' symbols and '='. Use only for base64 url modification by Hedgey
  /// </summary>
  /// <param name="base64UrlMod"></param>
  /// <returns></returns>
  static public string Shortify(string base64UrlMod)
    => regex.Replace(base64UrlMod, string.Empty);
  /// <summary>
  /// Each base64 of Snowflake ID has 11 symbols. And each of them has 1 '=' at the end.
  ///
  /// </summary>
  /// <param name="shortHash"></param>
  /// <returns></returns>
  static public string Expand(string shortHash)
  {
    const char eq = '=';
    int length = shortHash.Length;
    if (length == 12 && shortHash[length - 1] == eq)
      return shortHash;
    if (length >= 12)
      throw new ArgumentException($"\"{shortHash}\" is not a BlendedFlake ID hash");

    const int max = 11; // Hash length is 11 = 10 symbols + 1 symbol '='
    return new StringBuilder(shortHash)
      .Append('-', max - length)
      .Append(eq)
      .ToString();
  }
  static public bool TryParse(string hash, out ulong id)
  {
    id = 0;
    if (hash.Length > 12)
      return false;
    try
    {
      id = (ulong)Base64UrlMod.ToLong(hash);
      return true;
    }
    catch
    {
      return false;
    }
  }
}