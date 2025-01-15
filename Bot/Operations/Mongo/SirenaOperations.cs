using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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

  public IObservable<SirenRepresentation> Delete(long uid, ulong sid)
  {
    return DeleteSirenaDocument(sid)
      .CombineLatest(DeleteSirenaIdFromOwner(uid, sid), (x, y) => x);
  }

  private IObservable<SirenRepresentation> DeleteSirenaDocument(ulong sid)
  {
    var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sid);
    return Observable.FromAsync(() => sirens.FindOneAndDeleteAsync(sirenFilter));
  }

  private IObservable<UpdateResult> DeleteSirenaIdFromOwner(long uid, ulong sid)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserRepresentation>.Update.Pull(x => x.Owner, sid);
    return Observable.FromAsync(() => users.UpdateOneAsync(filter, userUpdate));
  }
  public IObservable<SirenRepresentation> Subscribe(long uid, ulong sid)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sid)
        & Builders<SirenRepresentation>.Filter.Ne(x => x.OwnerId, uid);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, uid);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filterSiren, addSubsription));
  }
  public IObservable<bool> Unsubscribe(long uid, ulong sid)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sid);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Listener, uid)
        .Pull(x => x.Responsible, uid) // Удаление из Responsible
        .PullFilter(x => x.Requests, _r => _r.UID == uid)
        .PullFilter(x => x.Muted, _m => _m.UID == uid);
    return Observable.FromAsync(() => sirens.UpdateOneAsync(filter, update)).Select(x => x.ModifiedCount != 0);
  }
  public IObservable<SirenRepresentation> Find(ulong sid)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sid);
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
  public IObservable<IEnumerable<SirenRepresentation>> GetAvailableForCallSirenas(long uid)
  {
    var query = sirens.AsQueryable()
      .Where(_sirena => (_sirena.OwnerId == uid || _sirena.Responsible.Any(_r => _r == uid))
            && (_sirena.Listener.Length - _sirena.Muted.Count(_m => _m.MutedUID == uid)) > 0);
    return Observable.FromAsync(() => query.ToListAsync());
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.AnyEq(x => x.Listener, uid);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync())
        .Catch((Exception _ex) =>
            {
              Console.WriteLine(_ex);
              throw _ex;
            });
  }
  public IObservable<SirenRepresentation> GetUserSirena(long uid, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return Observable.Throw<SirenRepresentation>(new ArgumentException("Serial number has to be positive", nameof(number)));

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return Observable.FromAsync(() => sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync());
  }
  /// <summary>
  /// Update sirena document in a database with new date of last call
  /// </summary>
  /// <param name="sid"></param>
  /// <param name="callInfo"></param>
  /// <returns></returns>
  public IObservable<SirenRepresentation> UpdateLastCall(ulong sid, SirenRepresentation.CallInfo callInfo)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sid);
    var update = Builders<SirenRepresentation>.Update.Set(x => x.LastCall, callInfo);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filter, update));
  }

  public IObservable<IRightsRequestOperation.Result> Send(ulong sid, long requesterId, string message)
    => Observable.FromAsync(() => RequestRightsAsync(sid, requesterId, message))
    .Select(_result => new IRightsRequestOperation.Result(_result.MatchedCount != 0, _result.ModifiedCount != 0));

  private Task<UpdateResult> RequestRightsAsync(ulong sid, long requesterId, string message)
  {
    var request = new SirenRepresentation.Request(requesterId, message);
    FilterDefinitionBuilder<SirenRepresentation> filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sid)
              & filterBuilder.Ne(x => x.OwnerId, requesterId)
              & !filterBuilder.AnyEq(x => x.Responsible, requesterId)
              & !filterBuilder.ElemMatch(x => x.Requests, r => r.UID == requesterId);
    var update = Builders<SirenRepresentation>.Update
              .AddToSet(x => x.Requests, request)
              .AddToSet(x => x.Listener, requesterId);

    return sirens.UpdateOneAsync(filter, update);
  }

  //Update function. Do not use for real database
  public IObservable<bool> UpdateDefault(ulong sirenaId)
  {

    FilterDefinitionBuilder<SirenRepresentation> filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Gt(s => s.SID, 500000);
    var update = Builders<SirenRepresentation>.Update.Set(s => s.SID, sirenaId);

    return Observable.FromAsync(() => sirens.UpdateOneAsync(filter, update))
    .Select(x => x.IsAcknowledged && x.IsModifiedCountAvailable && x.ModifiedCount > 0);
  }
}