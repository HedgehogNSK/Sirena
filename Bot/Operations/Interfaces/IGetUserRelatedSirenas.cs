using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;

public interface IGetUserRelatedSirenas
{
  IObservable<IEnumerable<SirenRepresentation>> GetAvailableForCallSirenas(long uid);
  IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long uid);
  IObservable<SirenRepresentation> GetUserSirena(long uid, int number);
  IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long uid);
}