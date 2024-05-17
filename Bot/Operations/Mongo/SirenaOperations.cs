using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class SirenaOperations : IDeleteSirenaOperation, ISubscribeToSirenaOperation, IUnsubscribeSirenaOperation, IFindSirenaOperation, IGetUserRelatedSirenas
{
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;

  public SirenaOperations(IMongoCollection<SirenRepresentation> sirenCollection
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
  public IObservable<SirenRepresentation> Subscribe(long uid, ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id) 
        & Builders<SirenRepresentation>.Filter.Ne(x=>x.OwnerId, uid);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, uid);
    return sirens.FindOneAndUpdateAsync(filterSiren, addSubsription).ToObservable();
  }
  public IObservable<bool> Unsubscribe(long uid, ObjectId id)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Listener, uid);
    return sirens.UpdateOneAsync(filter, update).ToObservable().Select(x => x.ModifiedCount != 0);
  }
  public IObservable<SirenRepresentation> Find(ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    return sirens.Find(filterSiren).FirstOrDefaultAsync().ToObservable();
  }

  public IObservable<List<SirenRepresentation>> Find(string keyPhrase)
  {
    var formatedKey = Regex.Escape(keyPhrase);
    var pattern = new Regex(formatedKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
    var bsonRegex = new BsonRegularExpression(pattern);
    var filter = Builders<SirenRepresentation>.Filter.Regex(x => x.Title, bsonRegex);
    return sirens.Find(filter).ToListAsync().ToObservable();
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return sirens.Find(filter).ToListAsync().ToObservable();
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.AnyEq(x => x.Listener, uid);
    return sirens.Find(filter).ToListAsync().ToObservable();
  }
  public IObservable<SirenRepresentation> GetUserSirena(long uid, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return Observable.Throw<SirenRepresentation>(new ArgumentException("Serial number has to be positive", nameof(number)));

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync().ToObservable();
  }

}