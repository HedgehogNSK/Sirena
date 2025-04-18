using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Reactive.Linq;

namespace Hedgey.Sirena.MongoDB.Operations;

public class UserOperations(IMongoCollection<UserData> users
  , IMongoCollection<SirenaData> sirens)
 : IUserEditOperations, IUserInfoOperations, IGetUserOverviewAsync
{
  private readonly IMongoCollection<UserData> users = users;
  private readonly IMongoCollection<SirenaData> sirens = sirens;

  public IObservable<UpdateState> CreateUser(long userID, long chatID)
  {
    var filter = Builders<UserData>.Filter.Eq(x => x.UID, userID);
    var update = Builders<UserData>.Update
        .SetOnInsert(x => x.UID, userID)
        .SetOnInsert(x => x.ChatID, chatID);
    var options = new UpdateOptions { IsUpsert = true };
    return Observable.FromAsync(() => users.UpdateOneAsync(filter, update, options))
      .Select(x =>
      {
        UpdateState result = UpdateState.Fail;

        if (x.IsAcknowledged) result = UpdateState.Success;
        if (x.MatchedCount == 1) result |= UpdateState.Exists;
        if (x.IsModifiedCountAvailable && x.ModifiedCount != 0) result |= UpdateState.Modified;
        return result;
      });
  }

  public IObservable<UserStatistics> Get(long uid) 
    => Observable.FromAsync(() => (this as IGetUserOverviewAsync).Get(uid));

  Task<UserStatistics> IGetUserOverviewAsync.Get(long uid)
  {
    var query = sirens.AsQueryable()
    .Where(_sirena => _sirena.OwnerId == uid
                      || _sirena.Listener.Any(x => x == uid)
                      || _sirena.Responsible.Any(x => x == uid))
    .GroupBy(keySelector: x => true,
      resultSelector: (_, _sirens) => new UserStatistics
      {
        Requests = _sirens.Sum(_sirena => (_sirena.OwnerId == uid && Mql.Exists(_sirena.Requests) ) ? _sirena.Requests.Length : 0),
        Responsible = _sirens.Sum(_sirena => (Mql.Exists(_sirena.Responsible) && _sirena.Responsible.Contains(uid)) ? 1 : 0),
        SirenasCount = _sirens.Sum(x => x.OwnerId == uid ? 1 : 0),
        Subscriptions = _sirens.Sum(_sirena => (Mql.Exists(_sirena.Listener) && _sirena.Listener.Contains(uid)) ? 1 : 0),
      });
      return query.FirstOrDefaultAsync();
  }
}