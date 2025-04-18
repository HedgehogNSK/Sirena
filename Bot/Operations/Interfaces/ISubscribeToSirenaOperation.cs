using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot.Operations;

public interface ISubscribeToSirenaOperation
{
  IObservable<SirenaData> Subscribe(long userId, ulong sirenaId);
}