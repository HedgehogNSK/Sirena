using System.Text;
using Hedgey.Extensions;
using Hedgey.Tools.BlendedFlake;
using Hedgey.Utilities;

namespace Hedgey.Debug;

static public class TestIDGenerator
{
  private static Generator generator;

  static public void SetGenerator(Generator generator) { TestIDGenerator.generator = generator; }
  static public void Display(ulong id)
  {
    string result = NotBase64URL.From(id); // Get Base64 URL Hedgey modification string
    Console.WriteLine($"Bits [Low Ending]: {Bit.ToString(id, false)}");
    string shortResult = HashUtilities.Shortify(result);
    string restoreResult = HashUtilities.Expand(shortResult);
    ulong number = (ulong)NotBase64URL.ToLong(restoreResult);
    var machineID = Generator.ComputeMachineID(id);
    var sequence = Generator.ComputeSequenceNumber(id);
    var timestamp = Generator.ComputeCreationTimestamp(id, generator.epoch);
    DateTimeOffset date = DateTimeOffset.FromUnixTimeMilliseconds((long)timestamp);
    Console.WriteLine($"Source ID:{id}\nBits [Low Ending]: {Bit.ToString(id, false)}\nBase64URLHM hash: {result} | Short variant:\t{shortResult}\nResotred hash:\t{restoreResult} | Restored id:\t{number}\nRestored bits: {Bit.ToString(number, false)}\nMachine ID: {machineID} | {sequence} | Date: {date}[{timestamp}]\n﹋﹋﹋﹋﹋");
    if (number != id)
      throw new InvalidOperationException($"ID conversion failed. Source ID:  {id}; Restored: {number}");
  }
  static public void TestBase64(string test)
  {
    var bytes = Encoding.ASCII.GetBytes(test);
    var hash = NotBase64URL.From(bytes);
    Console.WriteLine(hash);
  }
}