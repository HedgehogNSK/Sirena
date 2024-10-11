using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;

public interface ISubscribeToSirenaOperation
{
  IObservable<SirenRepresentation> Subscribe(long userId, ulong sirenaId);
}