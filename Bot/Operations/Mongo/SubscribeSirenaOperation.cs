using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Threading.Tasks;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class SubscribeSirenaOperation : ISubscribeToSirenaOperation
{
  private readonly IMongoCollection<SirenRepresentation> sirens;

  public SubscribeSirenaOperation(IMongoCollection<SirenRepresentation> sirens)
  {
    this.sirens = sirens;
  }
  public IObservable<SirenRepresentation> Subscribe(long uid, ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, uid);
    return sirens.FindOneAndUpdateAsync(filterSiren, addSubsription).ToObservable();
  }
}