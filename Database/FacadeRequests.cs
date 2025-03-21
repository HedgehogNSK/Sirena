using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.Database;

public class FacadeMongoDBRequests
{
  public readonly IMongoDatabase db;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;

  public FacadeMongoDBRequests(IMongoClient client)
  {
    db = client.GetDatabase("siren");
    sirens = db.GetCollection<SirenRepresentation>("sirens");
    users = db.GetCollection<UserRepresentation>("users");
  }
  public async Task<SirenRepresentation?> GetSirenaBySerialNumber(long uid, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return null;

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    var sirena = await sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync();

    return sirena;
  }
  public async Task<SirenRepresentation> GetSirenaById(ulong sid)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sid);
    var sirena = await sirens.Find(filterSiren).FirstOrDefaultAsync();
    return sirena;
  }

  public async Task<UserRepresentation> CreateUser(long userID, long chatID)
  {
    UserRepresentation user = new UserRepresentation
    {
      UID = userID,
      ChatID = chatID,
      Owner = [],
    };
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, user.UID);
    var update = Builders<UserRepresentation>.Update
        .SetOnInsert(x => x.UID, userID)
        .SetOnInsert(x => x.ChatID, chatID);
    var options = new UpdateOptions { IsUpsert = true };
    await users.UpdateOneAsync(filter, update, options);
    return user;
  }

  /// <summary>
  /// Give rights to user to call sirena
  /// </summary>
  /// <param name="uid">Owner id</param>
  /// <param name="sid">Sirena  id</param>
  /// <param name="duid">Target user</param>
  /// <returns></returns>
  public async Task<SirenRepresentation> SetUserResponsible(long uid, ulong sid, long duid)
  {
    var filter = Builders<SirenRepresentation>.Filter;
    var userFilter = filter.Eq(x => x.SID, sid)
                   & filter.Eq(x => x.OwnerId, uid);
    var update = Builders<SirenRepresentation>.Update
        .AddToSet(x => x.Responsible, duid)
        .PullFilter(x => x.Requests, x => x.UID == duid);
    var sirena = await sirens.FindOneAndUpdateAsync<SirenRepresentation>(userFilter, update);
    return sirena;
  }
  /// <summary>
  /// Take away right from user that was responsible for sirena
  /// </summary>
  /// <param name="uid">User ID of sirena owner</param>
  /// <param name="id">Sirena ID</param>
  /// <param name="ruid">User ID to revoke rights</param>
  /// <returns></returns>
  public async Task<SirenRepresentation> RevokeUserRights(long uid, ulong id, long ruid)
  {
    var filter = Builders<SirenRepresentation>.Filter;
    var userFilter = filter.Eq(x => x.SID, id)
                   & filter.Eq(x => x.OwnerId, uid);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Responsible, ruid);
    var sirena = await sirens.FindOneAndUpdateAsync<SirenRepresentation>(userFilter, update);
    return sirena;
  }

  internal async Task<SirenRepresentation> SetCallDate(ulong id, SirenRepresentation.CallInfo callInfo)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, id);
    var update = Builders<SirenRepresentation>.Update.Set(x => x.LastCall, callInfo);
    var sirena = await sirens.FindOneAndUpdateAsync(filter, update);
    return sirena;
  }

  internal async Task<SirenRepresentation> SetUserMute(long initiatorID, long targetID, ulong sirenaId)
  {
    var muted = new SirenRepresentation.MutedInfo(initiatorID,targetID);
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaId);
    var update = Builders<SirenRepresentation>.Update.AddToSet(x => x.Muted, muted);
    var options = new FindOneAndUpdateOptions<SirenRepresentation>
    {
      IsUpsert = true,
      ReturnDocument = ReturnDocument.After
    };
    var userUpdate = await sirens.FindOneAndUpdateAsync(filter, update, options);
    return userUpdate;
  }
  internal async Task<bool> IsPossibleToMute(long initiatorID, long targetID, ulong sirenaId)
  {
    var filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sirenaId)
    & (filterBuilder.Eq(x => x.OwnerId, targetID) | filterBuilder.AnyEq(x => x.Responsible, targetID))
    & filterBuilder.AnyEq(x => x.Listener, initiatorID);
    var anySirena = await sirens.Find(filter).AnyAsync();
    return anySirena;
  }

  internal async Task<SirenRepresentation> UnmuteUser(long initiatorID, long mutedUserID, ulong sirenaID)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, sirenaID);
    var update = Builders<SirenRepresentation>.Update.PullFilter(x => 
      x.Muted, _muted => _muted.UID == initiatorID && _muted.MutedUID == mutedUserID);

    var userUpdate = await sirens.FindOneAndUpdateAsync(filter, update);
    return userUpdate;
  }

  internal async Task<UpdateResult> RequestRightsForSirena(ulong sirenaId, long requesterId, string message)
  {
    var request = new SirenRepresentation.Request(requesterId, message);
    FilterDefinitionBuilder<SirenRepresentation> filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sirenaId)
              & filterBuilder.Ne(x => x.OwnerId, requesterId)
              & !filterBuilder.AnyEq(x => x.Responsible, requesterId);
    var update = Builders<SirenRepresentation>.Update
              .AddToSet(x => x.Requests, request)
              .AddToSet(x => x.Listener, requesterId);
    UpdateResult result = await sirens.UpdateOneAsync(filter, update);
    return result;
  }

  internal async Task<IEnumerable<SirenRepresentation>> GetSirenaByName(string searchKey)
  {
    var formatedKey = Regex.Escape(searchKey);
    var pattern = new Regex(formatedKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
    var bsonRegex = new BsonRegularExpression(pattern);
    var filter = Builders<SirenRepresentation>.Filter.Regex(x => x.Title, bsonRegex);
    var result = await sirens.Find(filter).ToListAsync();
    return result;
  }

  public async Task<List<SirenRepresentation>> GetOwnedSirenas(long uid)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    return await sirens.Find(filter).ToListAsync();
  }
  public async Task<SirenRepresentation> DeleteUsersSirena(long uid, ulong id)
  {
    await DeleteSirenaIdFromOwner(uid, id);
    return await DeleteSirenaDocument(id);
  }
  private async Task<SirenRepresentation> DeleteSirenaDocument(ulong id)
  {
    var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.SID, id);
    return await sirens.FindOneAndDeleteAsync(sirenFilter);
  }

  private async Task<UpdateResult> DeleteSirenaIdFromOwner(long uid, ulong sid)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserRepresentation>.Update.Pull<ulong>(x => x.Owner, sid);
    return await users.UpdateOneAsync(filter, userUpdate);
  }
}