using System.Reactive.Threading.Tasks;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class FindSirenaOperation : IFindSirenaOperation
{
  private readonly IMongoCollection<SirenRepresentation> sirens;

  public FindSirenaOperation(IMongoCollection<SirenRepresentation> sirenCollection)
  {
    this.sirens = sirenCollection;
  }

  public IObservable<SirenRepresentation> Find(ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    return sirens.Find(filterSiren).FirstOrDefaultAsync().ToObservable();
  }
}