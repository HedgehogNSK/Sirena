using Hedgey.Sirena.Database;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class UserOperations(IMongoCollection<UserRepresentation> users
  , IMongoCollection<SirenRepresentation> sirens)
 : IUserEditOperations, IUserInfoOperations
{
  private readonly IMongoCollection<UserRepresentation> users = users;
  private readonly IMongoCollection<SirenRepresentation> sirens = sirens;

  public IObservable<UpdateState> CreateUser(long userID, long chatID)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, userID);
    var update = Builders<UserRepresentation>.Update
        .SetOnInsert(x => x.UID, userID)
        .SetOnInsert(x => x.ChatID, chatID);
    UserRepresentation newUser = new() { UID = userID, ChatID = chatID };
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

  public IObservable<UserStatistics> Get(long userID)
  {
    var query = sirens.AsQueryable()
    .Where(_sirena => _sirena.OwnerId == userID
                      || _sirena.Listener.Any(x => x == userID)
                      || _sirena.Responsible.Any(x => x == userID))
    .GroupBy(keySelector: x => true,
      resultSelector: (_, _sirens) => new UserStatistics
      {
        SirenasCount = _sirens.Sum(x => x.OwnerId == userID ? 1 : 0),
        Subscriptions = _sirens.Sum(_sirena => (Mql.Exists(_sirena.Listener) && _sirena.Listener.Contains(userID)) ? 1 : 0),
        Responsible = _sirens.Sum(_sirena => (Mql.Exists(_sirena.Responsible) && _sirena.Responsible.Contains(userID)) ? 1 : 0)
      });

    return Observable.FromAsync(() => query.FirstOrDefaultAsync());
  }
}