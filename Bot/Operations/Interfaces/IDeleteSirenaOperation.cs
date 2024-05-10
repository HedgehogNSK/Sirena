using Hedgey.Sirena.Database;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot.Operations
{
  public interface IDeleteSirenaOperation
  {
    IObservable<SirenRepresentation> Delete(long uid, ObjectId id);
  }
}
