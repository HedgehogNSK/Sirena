using Hedgey.Extensions;

namespace Hedgey.Debug;

static public class TestIDGenerator
{
  static public void Display(ulong id)
  {
    string result = Converter.UlongToBase64URLHM(id); // Get Base64 URL Hedgey modification string
    string shortResult = BlendedflakeIDGenerator.ShortifyHash(result);
    string restoreResult = BlendedflakeIDGenerator.RestoreHash(shortResult);
    ulong number = (ulong)Converter.FromBase64URLHMToLong(shortResult);
    Console.WriteLine($"Source ID:{id}\nBits [Low Ending]: {Bit.ToString(id, false)}\nBase64URLHM hash: {result} | Short variant:\t{shortResult}\nResotred hash:\t{restoreResult} | Restored id:\t{number}\n﹋﹋﹋﹋﹋");
    if (number != id)
      throw new InvalidOperationException($"ID conversion failed. Source ID:  {id}; Restored: {number}");
  }
}