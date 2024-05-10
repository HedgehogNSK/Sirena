using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;
public interface IFindUserSirenasOperation
{
  IObservable<IEnumerable<SirenRepresentation>> Find(long uid);
   IObservable<SirenRepresentation> FindBySerialNumber(long uid, int number);
}
