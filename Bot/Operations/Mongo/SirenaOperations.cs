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
, IUpdateSirenaOperation, IRightsRequestOperation, IRightsManageOperation
{
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;

  public SirenaOperations(IMongoCollection<SirenRepresentation> sirenCollection
  , IMongoCollection<UserRepresentation> usersCollection)
  {
    this.sirens = sirenCollection;
    this.users = usersCollection;
  }

  public IObservable<SirenRepresentation> Delete(long userId, ulong sirenaId)
  {
    return DeleteSirenaDocument(sirenaId)
      .CombineLatest(DeleteSirenaIdFromOwner(userId, sirenaId), (x, y) => x);
  }

  private IObservable<SirenRepresentation> DeleteSirenaDocument(ulong sirenaId)
  {
    var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaId);
    return Observable.FromAsync(() => sirens.FindOneAndDeleteAsync(sirenFilter));
  }

  private IObservable<UpdateResult> DeleteSirenaIdFromOwner(long userId, ulong sirenaId)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, userId);
    var userUpdate = Builders<UserRepresentation>.Update.Pull(x => x.Owner, sirenaId);
    return Observable.FromAsync(() => users.UpdateOneAsync(filter, userUpdate));
  }
  public IObservable<SirenRepresentation> Subscribe(long userId, ulong sirenaId)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaId)
        & Builders<SirenRepresentation>.Filter.Ne(x => x.OwnerId, userId);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, userId);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filterSiren, addSubsription));
  }
  public IObservable<bool> Unsubscribe(long userId, ulong sirenaId)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaId);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Listener, userId)
        .Pull(x => x.Responsible, userId) // Удаление из Responsible
        .PullFilter(x => x.Requests, _r => _r.UID == userId)
        .PullFilter(x => x.Muted, _m => _m.UID == userId);
    return Observable.FromAsync(() => sirens.UpdateOneAsync(filter, update)).Select(x => x.ModifiedCount != 0);
  }
  public IObservable<SirenRepresentation> Find(ulong sirenaId)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaId);
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
  public IObservable<IEnumerable<SirenRepresentation>> GetAvailableForCallSirenas(long userId)
  {
    var query = sirens.AsQueryable()
      .Where(_sirena => (_sirena.OwnerId == userId || _sirena.Responsible.Any(_r => _r == userId))
            && (_sirena.Listener.Length - _sirena.Muted.Count(_m => _m.MutedUID == userId)) > 0);
    return Observable.FromAsync(() => query.ToListAsync());
  }

  public IObservable<IEnumerable<SirenRepresentation>> GetSirenasWithRequests(long userId)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, userId)
        & Builders<SirenRepresentation>.Filter.SizeGt(s => s.Requests, 0);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }

  public IObservable<IEnumerable<SirenRepresentation>> GetSubscriptions(long userId)
  {
    var filter = Builders<SirenRepresentation>.Filter.AnyEq(x => x.Listener, userId);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync())
        .Catch((Exception _ex) =>
            {
              Console.WriteLine(_ex);
              throw _ex;
            });
  }
  public IObservable<SirenRepresentation> GetUserSirena(long userId, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return Observable.Throw<SirenRepresentation>(new ArgumentException("Serial number has to be positive", nameof(number)));

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, userId);
    return Observable.FromAsync(() => sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync());
  }
  public IObservable<IEnumerable<SirenRepresentation>> GetUserSirenas(long userId)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, userId);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync())
        .Catch((Exception _ex) =>
            {
              Console.WriteLine(_ex);
              throw _ex;
            });
  }
  public IObservable<SirenRepresentation> GetUserSirenaOrNull(long userId, ulong sirenaId)
  {
    var filter = Builders<SirenRepresentation>.Filter;
    var filterSiren = filter.Eq(x => x.SID, sirenaId) & filter.Eq(x => x.OwnerId, userId);
    return Observable.FromAsync(() => sirens.Find(filterSiren).FirstOrDefaultAsync());
  }
  /// <summary>
  /// Update sirena document in a database with new date of last call
  /// </summary>
  /// <param name="sirenaId"></param>
  /// <param name="callInfo"></param>
  /// <returns></returns>
  public IObservable<SirenRepresentation> UpdateLastCall(ulong sirenaId, SirenRepresentation.CallInfo callInfo)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaId);
    var update = Builders<SirenRepresentation>.Update.Set(x => x.LastCall, callInfo);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filter, update));
  }

  public IObservable<IRightsRequestOperation.Result> Send(ulong sirenaId, long requestorId, string message)
    => Observable.FromAsync(() => RequestRightsAsync(sirenaId, requestorId, message))
    .Select(_result => new IRightsRequestOperation.Result(_result.MatchedCount != 0, _result.ModifiedCount != 0));

  private Task<UpdateResult> RequestRightsAsync(ulong sirenaId, long requesterId, string message)
  {
    var request = new SirenRepresentation.Request(requesterId, message);
    FilterDefinitionBuilder<SirenRepresentation> filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sirenaId)
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

  public IObservable<bool> Decline(ulong sirenaId, long requestorId)
  {
    var sirenaFilter = Builders<SirenRepresentation>.Filter.Eq(_sirena => _sirena.SID, sirenaId);
    var requestFilter = Builders<SirenRepresentation.Request>.Filter.Eq(s => s.UID, requestorId);
    var update = Builders<SirenRepresentation>.Update.PullFilter(s => s.Requests, requestFilter);

    return Observable.FromAsync(() => sirens.UpdateOneAsync(sirenaFilter, update))
    .Select(x => x.IsAcknowledged && x.IsModifiedCountAvailable && x.ModifiedCount > 0);
  }
}