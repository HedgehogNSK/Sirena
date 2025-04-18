using Hedgey.Sirena.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.MongoDB;

public class FacadeMongoDBRequests
{
  public readonly IMongoDatabase db;
  private readonly IMongoCollection<SirenaData> sirens;
  private readonly IMongoCollection<UserData> users;

  public FacadeMongoDBRequests(IMongoClient client)
  {
    db = client.GetDatabase("siren");
    sirens = db.GetCollection<SirenaData>("sirens");
    users = db.GetCollection<UserData>("users");
  }
  public async Task<SirenaData?> GetSirenaBySerialNumber(long uid, int number)
  {
    --number;// switch number to id
    if (number < 0)
      return null;

    var filterSiren = Builders<SirenaData>.Filter.Eq(x => x.OwnerId, uid);
    var sirena = await sirens.Find(filterSiren).Skip(number).FirstOrDefaultAsync();

    return sirena;
  }
  public async Task<SirenaData> GetSirenaById(ulong sid)
  {
    var filterSiren = Builders<SirenaData>.Filter.Eq(x => x.SID, sid);
    var sirena = await sirens.Find(filterSiren).FirstOrDefaultAsync();
    return sirena;
  }

  public async Task<UserData> CreateUser(long userID, long chatID)
  {
    UserData user = new UserData
    {
      UID = userID,
      ChatID = chatID,
      Owner = [],
    };
    var filter = Builders<UserData>.Filter.Eq(x => x.UID, user.UID);
    var update = Builders<UserData>.Update
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
  public async Task<SirenaData> SetUserResponsible(long uid, ulong sid, long duid)
  {
    var filter = Builders<SirenaData>.Filter;
    var userFilter = filter.Eq(x => x.SID, sid)
                   & filter.Eq(x => x.OwnerId, uid);
    var update = Builders<SirenaData>.Update
        .AddToSet(x => x.Responsible, duid)
        .PullFilter(x => x.Requests, x => x.UID == duid);
    var sirena = await sirens.FindOneAndUpdateAsync<SirenaData>(userFilter, update);
    return sirena;
  }
  /// <summary>
  /// Take away right from user that was responsible for sirena
  /// </summary>
  /// <param name="uid">User ID of sirena owner</param>
  /// <param name="id">Sirena ID</param>
  /// <param name="ruid">User ID to revoke rights</param>
  /// <returns></returns>
  public async Task<SirenaData> RevokeUserRights(long uid, ulong id, long ruid)
  {
    var filter = Builders<SirenaData>.Filter;
    var userFilter = filter.Eq(x => x.SID, id)
                   & filter.Eq(x => x.OwnerId, uid);
    var update = Builders<SirenaData>.Update.Pull(x => x.Responsible, ruid);
    var sirena = await sirens.FindOneAndUpdateAsync<SirenaData>(userFilter, update);
    return sirena;
  }

  internal async Task<SirenaData> SetUserMute(long initiatorID, long targetID, ulong sirenaId)
  {
    var muted = new SirenaData.MutedInfo(initiatorID,targetID);
    var filter = Builders<SirenaData>.Filter.Eq(x => x.SID, sirenaId);
    var update = Builders<SirenaData>.Update.AddToSet(x => x.Muted, muted);
    var options = new FindOneAndUpdateOptions<SirenaData>
    {
      IsUpsert = true,
      ReturnDocument = ReturnDocument.After
    };
    var userUpdate = await sirens.FindOneAndUpdateAsync(filter, update, options);
    return userUpdate;
  }
  internal async Task<bool> IsPossibleToMute(long initiatorID, long targetID, ulong sirenaId)
  {
    var filterBuilder = Builders<SirenaData>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sirenaId)
    & (filterBuilder.Eq(x => x.OwnerId, targetID) | filterBuilder.AnyEq(x => x.Responsible, targetID))
    & filterBuilder.AnyEq(x => x.Listener, initiatorID);
    var anySirena = await sirens.Find(filter).AnyAsync();
    return anySirena;
  }

  internal async Task<SirenaData> UnmuteUser(long initiatorID, long mutedUserID, ulong sirenaID)
  {
    var filter = Builders<SirenaData>.Filter.Eq(x => x.SID, sirenaID);
    var update = Builders<SirenaData>.Update.PullFilter(x => 
      x.Muted, _muted => _muted.UID == initiatorID && _muted.MutedUID == mutedUserID);

    var userUpdate = await sirens.FindOneAndUpdateAsync(filter, update);
    return userUpdate;
  }

  internal async Task<UpdateResult> RequestRightsForSirena(ulong sirenaId, long requesterId, string message)
  {
    var request = new SirenaData.Request(requesterId, message);
    FilterDefinitionBuilder<SirenaData> filterBuilder = Builders<SirenaData>.Filter;
    var filter = filterBuilder.Eq(x => x.SID, sirenaId)
              & filterBuilder.Ne(x => x.OwnerId, requesterId)
              & !filterBuilder.AnyEq(x => x.Responsible, requesterId);
    var update = Builders<SirenaData>.Update
              .AddToSet(x => x.Requests, request)
              .AddToSet(x => x.Listener, requesterId);
    UpdateResult result = await sirens.UpdateOneAsync(filter, update);
    return result;
  }

  internal async Task<IEnumerable<SirenaData>> GetSirenaByName(string searchKey)
  {
    var formatedKey = Regex.Escape(searchKey);
    var pattern = new Regex(formatedKey, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
    var bsonRegex = new BsonRegularExpression(pattern);
    var filter = Builders<SirenaData>.Filter.Regex(x => x.Title, bsonRegex);
    var result = await sirens.Find(filter).ToListAsync();
    return result;
  }

  public async Task<List<SirenaData>> GetOwnedSirenas(long uid)
  {
    var filter = Builders<SirenaData>.Filter.Eq(x => x.OwnerId, uid);
    return await sirens.Find(filter).ToListAsync();
  }
  public async Task<SirenaData> DeleteUsersSirena(long uid, ulong id)
  {
    await DeleteSirenaIdFromOwner(uid, id);
    return await DeleteSirenaDocument(id);
  }
  private async Task<SirenaData> DeleteSirenaDocument(ulong id)
  {
    var sirenFilter = Builders<SirenaData>.Filter.Eq(x => x.SID, id);
    return await sirens.FindOneAndDeleteAsync(sirenFilter);
  }

  private async Task<UpdateResult> DeleteSirenaIdFromOwner(long uid, ulong sid)
  {
    var filter = Builders<UserData>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserData>.Update.Pull<ulong>(x => x.Owner, sid);
    return await users.UpdateOneAsync(filter, userUpdate);
  }
}