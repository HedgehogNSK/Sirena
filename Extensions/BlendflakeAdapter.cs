using Hedgey.Blendflake;
using Hedgey.Extensions;

namespace Hedge.Sirena.ID;

public class BlendflakeAdapter : Generator, IIDGenerator
{
  public BlendflakeAdapter(long epoch, ushort machineID) : base(epoch, machineID)
  {
  }
}