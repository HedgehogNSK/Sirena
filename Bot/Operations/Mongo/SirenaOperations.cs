using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class SirenaOperations : IDeleteSirenaOperation
, ISubscribeToSirenaOperation, IUnsubscribeSirenaOperation
, IFindSirenaOperation, IGetUserRelatedSirenas
, IUpdateSirenaOperation, IRightsRequestOperation
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
    return Observable.FromAsync(() => sirens.FindOneAndDeleteAsync(sirenFilter));
  }

  public IObservable<UpdateResult> DeleteSirenaIdFromOwner(long uid, ObjectId id)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserRepresentation>.Update.Pull(x => x.Owner, id);
    return Observable.FromAsync(() => users.UpdateOneAsync(filter, userUpdate));
  }
  public IObservable<SirenRepresentation> Subscribe(long uid, ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id)
        & Builders<SirenRepresentation>.Filter.Ne(x => x.OwnerId, uid);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, uid);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filterSiren, addSubsription));
  }
  public IObservable<bool> Unsubscribe(long uid, ObjectId id)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Listener, uid)
        .Pull(x => x.Responsible, uid) // Удаление из Responsible
        .PullFilter(x => x.Requests, r => r.UID == uid);;
    return Observable.FromAsync(() => sirens.UpdateOneAsync(filter, update)).Select(x => x.ModifiedCount != 0);
  }
  public IObservable<SirenRepresentation> Find(ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    return Observable.FromAsync(() => sirens.Find(filterSiren).FirstOrDefaultAsync());
  }

  public IObservable<List<SirenRepresentation>> Find(string keyPhrase)
  {
    var formatedKey = Regex.Escape(keyPhrase);
    var pattern = new Regex(formatedKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
    var bsonRegex = new BsonRegularExpression(pattern);
    var filter = Builders<SirenRepresentation>.Filter.Regex(x => x.Title, bsonRegex);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.AnyEq(x => x.Listener, uid);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }
  public IObservable<SirenRepresentation> GetUserSirena(long uid, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return Observable.Throw<SirenRepresentation>(new ArgumentException("Serial number has to be positive", nameof(number)));

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return Observable.FromAsync(() => sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync());
  }

  public IObservable<SirenRepresentation> UpdateLastCall(ObjectId sirenId, SirenRepresentation.CallInfo callInfo)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, sirenId);
    var update = Builders<SirenRepresentation>.Update.Set(x => x.LastCall, callInfo);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filter, update));
  }

  public IObservable<IRightsRequestOperation.Result> Send(ObjectId sirenaId, long requesterId, string message)
    => Observable.FromAsync(() => RequestRightsAsync(sirenaId, requesterId, message))
    .Select(_result => new IRightsRequestOperation.Result(_result.MatchedCount!=0, _result.ModifiedCount!=0));

  private Task<UpdateResult> RequestRightsAsync(ObjectId sirenaId, long requesterId, string message)
  {
    var request = new SirenRepresentation.Request(requesterId, message);
    FilterDefinitionBuilder<SirenRepresentation> filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.Id, sirenaId)
              & filterBuilder.Ne(x => x.OwnerId, requesterId)
              & !filterBuilder.AnyEq(x => x.Responsible, requesterId)
              & !filterBuilder.ElemMatch(x => x.Requests, r => r.UID == requesterId);
    var update = Builders<SirenRepresentation>.Update
              .AddToSet(x => x.Requests, request)
              .AddToSet(x => x.Listener, requesterId);

    return sirens.UpdateOneAsync(filter, update);
  }
}