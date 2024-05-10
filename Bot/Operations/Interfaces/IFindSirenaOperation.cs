using Hedgey.Sirena.Database;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot.Operations;
public interface IFindSirenaOperation
{
  IObservable<SirenRepresentation> Find(ObjectId id);
  IObservable<List<SirenRepresentation>> Find(string keyPhrase);
}