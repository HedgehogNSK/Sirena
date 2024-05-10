using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class DeleteSirenaOperation : IDeleteSirenaOperation
{
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;

  public DeleteSirenaOperation(IMongoCollection<SirenRepresentation> sirenCollection
  , IMongoCollection<UserRepresentation> usersCollection)
  {
    this.sirens = sirenCollection;
    this.users = usersCollection;
  }

  public IObservable<SirenRepresentation> Delete(long uid, ObjectId id)
  {
    return DeleteSirenaDocument(id)
      .CombineLatest(DeleteSirenaIdFromOwner(uid, id), (x, y) => x);
  }

  public IObservable<SirenRepresentation> DeleteSirenaDocument(ObjectId id)
  {
    var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    return sirens.FindOneAndDeleteAsync(sirenFilter).ToObservable();
    // return await sirens.Find(sirenFilter).FirstOrDefaultAsync();
  }

  public IObservable<UpdateResult> DeleteSirenaIdFromOwner(long uid, ObjectId id)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserRepresentation>.Update.Pull(x => x.Owner, id);
    return users.UpdateOneAsync(filter, userUpdate).ToObservable();
  }
}