using Hedgey.Extensions;
using Hedgey.Tools.BlendedFlake;
using Hedgey.Utilities;

namespace Hedgey.Debug;

static public class TestIDGenerator
{
  static public void Display(ulong id)
  {
    string result = Base64UrlMod.FromLong(id); // Get Base64 URL Hedgey modification string
    Console.WriteLine($"Bits [Low Ending]: {Bit.ToString(id, false)}");
    string shortResult = HashUtilities.Shortify(result);
    string restoreResult = HashUtilities.Expand(shortResult);
    ulong number = (ulong)Base64UrlMod.ToLong(restoreResult);
    Console.WriteLine($"Source ID:{id}\nBits [Low Ending]: {Bit.ToString(id, false)}\nBase64URLHM hash: {result} | Short variant:\t{shortResult}\nResotred hash:\t{restoreResult} | Restored id:\t{number}\nRestored bits: {Bit.ToString(number, false)}\n﹋﹋﹋﹋﹋");
    if (number != id)
      throw new InvalidOperationException($"ID conversion failed. Source ID:  {id}; Restored: {number}");
  }
}