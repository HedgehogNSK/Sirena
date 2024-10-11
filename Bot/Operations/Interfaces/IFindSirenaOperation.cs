using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations;
public interface IFindSirenaOperation
{
  IObservable<SirenRepresentation> Find(ulong id);
  IObservable<List<SirenRepresentation>> Find(string keyPhrase);
}