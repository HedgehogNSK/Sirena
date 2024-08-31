namespace Hedgey.Extensions;

public class SnowflakeGenerator : IIDGenerator
{
  private readonly long _epoch;
  private readonly ulong _machineID;
  private uint _sequence;
  private long _lastTimestamp;
  private const int ID_BITS = 10;
  private const int SEQUENCE_BITS = 12;
  private const int ID_SHIFT = SEQUENCE_BITS;
  private const int TIMESTAMP_SHIFT = SEQUENCE_BITS + ID_BITS;
  private const int MAX_ID = (1 << ID_BITS) - 1; // 1023
  private const uint MAX_SEQUENCE_LENGTH = (1 << SEQUENCE_BITS) - 1; // 4095

  public SnowflakeGenerator(long epoch, uint machineID)
  {
    if (machineID < 0 || machineID > MAX_ID)
      throw new ArgumentOutOfRangeException(nameof(machineID), $"Machine ID must be between 0 and {MAX_ID}");

    _epoch = epoch;
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
      _sequence = MAX_SEQUENCE_LENGTH;

      return ((ulong)(_lastTimestamp - _epoch) << TIMESTAMP_SHIFT)
          | _machineID << ID_SHIFT
          | _sequence;
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
  ///Uncomment when there will be a lot of request per milisecond
  // static SnowflakeGenerator()
  // {
  //   StartTimer();
  // }
  // static void StartTimer()
  // {
  //   Task.Run(Monitoring);
  //   async void Monitoring()
  //   {
  //     while (true)
  //     {
  //       _timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
  //       await Task.Delay(1);
  //     }
  //   }
  // }
}