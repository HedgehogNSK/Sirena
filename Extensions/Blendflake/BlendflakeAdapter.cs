using Hedgey.Blendflake;
using Hedgey.Sirena.ID;

namespace Hedgey.Extensions.Blendflake;

public class BlendflakeAdapter : Generator, IIDGenerator
{
  public BlendflakeAdapter(long epoch, ushort machineID) : base(epoch, machineID)
  {
  }
}