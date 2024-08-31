using System.Text;
using System.Text.RegularExpressions;

namespace Hedgey.Extensions;

public class BlendedflakeIDGenerator : IIDGenerator
{
  private readonly ulong _epoch;
  private readonly ushort _machineID;
  private uint _sequence;
  private long _lastTimestamp;
  private const int ID_BITS = 10;
  private const int SEQUENCE_BITS = 12;
  private const int DATE_BITS = 42;
  private const int MAX_ID = (1 << ID_BITS) - 1; // 1023
  private const uint MAX_SEQUENCE_LENGTH = (1 << SEQUENCE_BITS) - 1; // 4095
  public BlendedflakeIDGenerator(long epoch, ushort machineID)
  {
    if (machineID < 0 || machineID > MAX_ID)
      throw new ArgumentOutOfRangeException(nameof(machineID), $"Machine ID must be between 0 and {MAX_ID}");

    _epoch = (ulong)epoch;
    _machineID = machineID;
    _sequence = 0;
    _lastTimestamp = -1L;
  }
  public ulong Get()
  {
    lock (this)
    {
      long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); ;
      if (timestamp < _lastTimestamp)
        throw new InvalidOperationException("Clock moved backwards. Refusing to generate id for a while.");

      if (timestamp != _lastTimestamp)
      {
        _sequence = 0;
        _lastTimestamp = timestamp;
      }
      else
      {
        _sequence = (_sequence + 1) & MAX_SEQUENCE_LENGTH;
        if (_sequence == 0)
          _lastTimestamp = WaitForNextMillisecond(_lastTimestamp);
      }
      if (_sequence >= 3)
        Console.WriteLine(_sequence);

      const int seqSlitBit = 4;  
      ulong result =  _sequence >> seqSlitBit << ID_BITS;
      result |= _machineID;
      result <<= seqSlitBit;
      result |= _sequence & 0xF;
      result <<= DATE_BITS;
      result |= ((ulong)_lastTimestamp - _epoch) & 0x3FFFFFFFFFF;
      return result;
    }
  }
  private long GetCurrentTimestamp()
    => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

  static Regex regex = new Regex("-*=");
  /// <summary>
  /// Remove last consequent 'A' symbols and '='. 
  /// </summary>
  /// <param name="base64hash"></param>
  /// <returns></returns>
  static public string ShortifyHash(string base64hash)
  {
    base64hash = regex.Replace(base64hash, string.Empty);
    return base64hash;
  }
  /// <summary>
  /// Each base64 of Snowflake ID has 11 symbols. And each of them has 1 '=' at the end.
  /// 
  /// </summary>
  /// <param name="shortSnowlakeHash"></param>
  /// <returns></returns>
  static public string RestoreHash(string shortSnowlakeHash)
  {
    const int max = 11; // Hash length is 11 = 10 symbols + 1 symbol '='
    int hashLenght = shortSnowlakeHash.Length;
    StringBuilder builder = new StringBuilder(shortSnowlakeHash);
    for (int i = 0; i != max - hashLenght; ++i)
    {
      builder.Append('-');
    }
    builder.Append('=');
    return builder.ToString();
  }
  private long WaitForNextMillisecond(long lastTimestamp)
  {
    return Task.Run(Wait4NextMillisecondAsync).GetAwaiter().GetResult();

    async Task<long> Wait4NextMillisecondAsync()
    {
      long timestamp;
      do
      {
        await Task.Delay(1);
        timestamp = GetCurrentTimestamp();
      } while (timestamp <= lastTimestamp);
      return timestamp;
    }
  }
}