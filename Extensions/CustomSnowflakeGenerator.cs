namespace Hedgey.Extensions;

public class CustomSnowflakeGenerator : IIDGenerator
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

  public CustomSnowflakeGenerator(long epoch, ushort machineID)
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
      ulong result = _sequence >> 3;
      result <<= ID_BITS;
      result |= _machineID;
      result <<= 3;
      result |= _sequence & 0x7;
      result <<= DATE_BITS;
      result |= ((ulong)_lastTimestamp - _epoch) & 0x3FFFFFFFFFF; // mask 0x3FFFFFFFFFF = 1<<DATE_BITS -1
      return result;
      // ulong temp = (ulong)_lastTimestamp - _epoch;
      // temp &= 0x3FFFFFFFFFF;
      // temp |= _sequence << SEQUENCE_SHIFT;
      // temp |= _machineID << ID_SHIFT;
      //return temp;
    }
  }

  private long GetCurrentTimestamp()
    => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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