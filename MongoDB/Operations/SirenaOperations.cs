using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.MongoDB.Operations;

public class SirenaOperations : IDeleteSirenaOperation
, ISubscribeToSirenaOperation, IUnsubscribeSirenaOperation
, IFindSirenaOperation, IGetUserRelatedSirenas
, IUpdateSirenaOperation, IRightsRequestOperation, IRightsManageOperation
, ISirenaActivationOperation
{
  private readonly IMongoCollection<SirenaData> sirens;
  private readonly IMongoCollection<UserData> users;
  private readonly IMongoCollection<SirenaActivation> calls;

  public SirenaOperations(IMongoCollection<SirenaData> sirenCollection
  , IMongoCollection<UserData> usersCollection
  , IMongoCollection<SirenaActivation> callsCollection)
  {
    this.sirens = sirenCollection;
    this.users = usersCollection;
    this.calls = callsCollection;
  }
  public IObservable<SirenaActivation> LogInfo(ulong sirenaId, long userId)
  {
    var call = new SirenaActivation(sirenaId, userId);

    return calls.InsertOneAsync(call).ToObservable()
      .Select(_ => call);
  }
  public IObservable<bool> SetReceivers(SirenaActivation call, IEnumerable<long> receiversIds)
  {
    SirenaActivation.Receiver[] receivers = receiversIds
      .Select(_userId => new SirenaActivation.Receiver(_userId))
      .ToArray();
    var filter = Builders<SirenaActivation>.Filter.Eq(x => x.Id, call.Id);
    var update = Builders<SirenaActivation>.Update.Set(x => x.Receivers, receivers);
    return calls.UpdateOneAsync(filter, update).ToObservable()
      .SelectMany(_result => UpdateLastCall(call)
      .Select(_ => _result.IsAcknowledged && _result.IsModifiedCountAvailable));
  }
  public IObservable<SirenaData> Delete(long userId, ulong sirenaId)
  {
    return DeleteSirenaDocument(sirenaId)
      .CombineLatest(DeleteSirenaIdFromOwner(userId, sirenaId), (x, y) => x);
  }

  private IObservable<SirenaData> DeleteSirenaDocument(ulong sirenaId)
  {
    var sirenFilter = Builders<SirenaData>.Filter.Eq(x => x.SID, sirenaId);
    return Observable.FromAsync(() => sirens.FindOneAndDeleteAsync(sirenFilter));
  }

  private IObservable<UpdateResult> DeleteSirenaIdFromOwner(long userId, ulong sirenaId)
  {
    var filter = Builders<UserData>.Filter.Eq(x => x.UID, userId);
    var userUpdate = Builders<UserData>.Update.Pull(x => x.Owner, sirenaId);
    return Observable.FromAsync(() => users.UpdateOneAsync(filter, userUpdate));
  }
  public IObservable<SirenaData> Subscribe(long userId, ulong sirenaId)
  {
    var filterSiren = Builders<SirenaData>.Filter.Eq(x => x.SID, sirenaId)
        & Builders<SirenaData>.Filter.Ne(x => x.OwnerId, userId);
    var addSubsription = Builders<SirenaData>.Update.AddToSet(x => x.Listener, userId);
    return Observable.FromAsync(() => sirens.FindOneAndUpdateAsync(filterSiren, addSubsription));
  }
  public IObservable<bool> Unsubscribe(long userId, ulong sirenaId)
  {
    var filter = Builders<SirenaData>.Filter.Eq(x => x.SID, sirenaId);
    var update = Builders<SirenaData>.Update.Pull(x => x.Listener, userId)
        .Pull(x => x.Responsible, userId) // Удаление из Responsible
        .PullFilter(x => x.Requests, _r => _r.UID == userId)
        .PullFilter(x => x.Muted, _m => _m.UID == userId);
    return Observable.FromAsync(() => sirens.UpdateOneAsync(filter, update)).Select(x => x.ModifiedCount != 0);
  }
  public IObservable<SirenaData> Find(ulong sirenaId)
  {
    var filterSiren = Builders<SirenaData>.Filter.Eq(x => x.SID, sirenaId);
    return Observable.FromAsync(() => sirens.Find(filterSiren).FirstOrDefaultAsync());
  }

  public IObservable<List<SirenaData>> Find(string keyPhrase)
  {
    var formatedKey = Regex.Escape(keyPhrase);
    var pattern = new Regex(formatedKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
    var bsonRegex = new BsonRegularExpression(pattern);
    var filter = Builders<SirenaData>.Filter.Regex(x => x.Title, bsonRegex);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }
  public IObservable<IEnumerable<SirenaData>> GetAvailableForCallSirenas(long userId)
  {
    var query = sirens.AsQueryable()
      .Where(_sirena => (_sirena.OwnerId == userId || _sirena.Responsible.Any(_r => _r == userId))
            && (_sirena.Listener.Length - _sirena.Muted.Count(_m => _m.MutedUID == userId)) > 0);
    return Observable.FromAsync(() => query.ToListAsync());
  }

  public IObservable<IEnumerable<SirenaData>> GetSirenasWithRequests(long userId)
  {
    var filter = Builders<SirenaData>.Filter.Eq(x => x.OwnerId, userId)
        & Builders<SirenaData>.Filter.SizeGt(s => s.Requests, 0);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync());
  }

  public IObservable<IEnumerable<SirenaData>> GetSubscriptions(long userId)
  {
    var filter = Builders<SirenaData>.Filter.AnyEq(x => x.Listener, userId);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync())
        .Catch((Exception _ex) =>
            {
              Console.WriteLine(_ex);
              throw _ex;
            });
  }
  public IObservable<SirenaData> GetUserSirena(long userId, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return Observable.Throw<SirenaData>(new ArgumentException("Serial number has to be positive", nameof(number)));

    var filterSiren = Builders<SirenaData>.Filter.Eq(x => x.OwnerId, userId);
    return Observable.FromAsync(() => sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync());
  }
  public IObservable<IEnumerable<SirenaData>> GetUserSirenas(long userId)
  {
    var filter = Builders<SirenaData>.Filter.Eq(x => x.OwnerId, userId);
    return Observable.FromAsync(() => sirens.Find(filter).ToListAsync())
        .Catch((Exception _ex) =>
            {
              Console.WriteLine(_ex);
              throw _ex;
            });
  }
  public IObservable<SirenaData> GetUserSirenaOrNull(long userId, ulong sirenaId)
  {
    var filter = Builders<SirenaData>.Filter;
    var filterSiren = filter.Eq(x => x.SID, sirenaId) & filter.Eq(x => x.OwnerId, userId);
    return Observable.FromAsync(() => sirens.Find(filterSiren).FirstOrDefaultAsync());
  }
  /// <summary>
  /// Update sirena document in a database with new date of last call
  /// </summary>
  /// <param name="sirenaId"></param>
  /// <param name="callInfo"></param>
  /// <returns></returns>
  private IObservable<SirenaData> UpdateLastCall(SirenaActivation callInfo)
  {
    SirenaData.CallInfo lastCallInfo = new(callInfo);
    var filter = Builders<SirenaData>.Filter.Eq(_sirena => _sirena.SID, callInfo.SirenaId);
    var update = Builders<SirenaData>.Update.Set(_sirena => _sirena.LastCall, lastCallInfo);
    return sirens.FindOneAndUpdateAsync(filter, update).ToObservable();
  }

  public IObservable<IRightsRequestOperation.Result> Send(ulong sirenaId, long requestorId, string message)
    => Observable.FromAsync(() => RequestRightsAsync(sirenaId, requestorId, message))
    .Select(_result => new IRightsRequestOperation.Result(_result.MatchedCount != 0, _result.ModifiedCount != 0));

  private Task<UpdateResult> RequestRightsAsync(ulong sirenaId, long requesterId, string message)
  {
    var request = new SirenaData.Request(requesterId, message);
    FilterDefinitionBuilder<SirenaData> filterBuilder = Builders<SirenaData>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sirenaId)
              & filterBuilder.Ne(x => x.OwnerId, requesterId)
              & !filterBuilder.AnyEq(x => x.Responsible, requesterId)
              & !filterBuilder.ElemMatch(x => x.Requests, r => r.UID == requesterId);
    var update = Builders<SirenaData>.Update
              .AddToSet(x => x.Requests, request)
              .AddToSet(x => x.Listener, requesterId);

    return sirens.UpdateOneAsync(filter, update);
  }

  public IObservable<SirenaActivation> SetReaction(ObjectId callId, long userId, int emojiCode)
  {
    var filter = Builders<SirenaActivation>.Filter.Eq(_call => _call.Id, callId)
      & Builders<SirenaActivation>.Filter.ElemMatch(_call => _call.Receivers, x => x.UserId == userId);
    UpdateDefinition<SirenaActivation> update;
    if (emojiCode > 0)
      update = Builders<SirenaActivation>.Update.Set(_call => _call.Receivers.FirstMatchingElement().Reaction, emojiCode);
    else
      update = Builders<SirenaActivation>.Update.Unset(_call => _call.Receivers.FirstMatchingElement().Reaction);
    var sort = Builders<SirenaActivation>.Sort.Descending(_call => _call.Date);
    var options = new FindOneAndUpdateOptions<SirenaActivation>()
    {
      Sort = sort
    };
    return Observable.FromAsync(() => calls.FindOneAndUpdateAsync(filter, update, options));
  }
  //Update function. Do not use for real database
  public IObservable<bool> UpdateDefault(ulong sirenaId)
  {

    FilterDefinitionBuilder<SirenaData> filterBuilder = Builders<SirenaData>.Filter;
    var filter = filterBuilder.Gt(s => s.SID, 500000);
    var update = Builders<SirenaData>.Update.Set(s => s.SID, sirenaId);

    return Observable.FromAsync(() => sirens.UpdateOneAsync(filter, update))
    .Select(x => x.IsAcknowledged && x.IsModifiedCountAvailable && x.ModifiedCount > 0);
  }

  public IObservable<bool> Decline(ulong sirenaId, long requestorId)
  {
    var sirenaFilter = Builders<SirenaData>.Filter.Eq(_sirena => _sirena.SID, sirenaId);
    var requestFilter = Builders<SirenaData.Request>.Filter.Eq(s => s.UID, requestorId);
    var update = Builders<SirenaData>.Update.PullFilter(s => s.Requests, requestFilter);

    return Observable.FromAsync(() => sirens.UpdateOneAsync(sirenaFilter, update))
    .Select(x => x.IsAcknowledged && x.IsModifiedCountAvailable && x.ModifiedCount > 0);
  }
}