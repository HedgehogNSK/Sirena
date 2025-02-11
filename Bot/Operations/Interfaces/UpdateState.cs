namespace Hedgey.Sirena.Bot.Operations;
[Flags]
public enum UpdateState
{
  Fail = 0,
  Success = 1,
  Exists = 1 << 1,
  Modified = 1 << 2,
  Upserted = Success | Modified,
  Updated = Success | Exists | Modified
}