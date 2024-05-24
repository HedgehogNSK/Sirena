using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.Database;

public class FacadeMongoDBRequests
{
  public readonly IMongoDatabase db;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;

  public FacadeMongoDBRequests(MongoClient client)
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
  public async Task<SirenRepresentation> GetSirenaById(ObjectId id)
  {
    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var sirena = await sirens.Find(filterSiren).FirstOrDefaultAsync();
    return sirena;
  }

  public async Task<ObjectId> GetSirenaId(long uid, string param)
  {
    if (!ObjectId.TryParse(param, out ObjectId id))
      if (int.TryParse(param, out int number))
      {
        var sirena = await GetSirenaBySerialNumber(uid, number);
        if (sirena != null)
          id = sirena.Id;
        else
        {
          const string failMessageText = "You don't have this *sirena*";
          //ChatId for personal chat is equal to userId
          Program.botProxyRequests.Send(uid, failMessageText);
        }
      }
    return id;
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
  /// <param name="id">Sirena  id</param>
  /// <param name="duid">Target user</param>
  /// <returns></returns>
  public async Task<SirenRepresentation> SetUserResponsible(long uid, ObjectId id, long duid)
  {
    var filter = Builders<SirenRepresentation>.Filter;
    var userFilter = filter.Eq(x => x.Id, id)
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
  public async Task<SirenRepresentation> RevokeUserRights(long uid, ObjectId id, long ruid)
  {
    var filter = Builders<SirenRepresentation>.Filter;
    var userFilter = filter.Eq(x => x.Id, id)
                   & filter.Eq(x => x.OwnerId, uid);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Responsible, ruid);
    var sirena = await sirens.FindOneAndUpdateAsync<SirenRepresentation>(userFilter, update);
    return sirena;
  }

  internal async Task<SirenRepresentation> SetCallDate(ObjectId id, SirenRepresentation.CallInfo callInfo)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var update = Builders<SirenRepresentation>.Update.Set(x => x.LastCall, callInfo);
    var sirena = await sirens.FindOneAndUpdateAsync(filter, update);
    return sirena;
  }

  internal async Task<UserRepresentation> SetUserMute(long initiatorID, long targetID, ObjectId sirenaId)
  {
    var muted = new UserRepresentation.MuteInfo(targetID, sirenaId);
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, initiatorID);
    var update = Builders<UserRepresentation>.Update.AddToSet(x => x.Muted, muted);
    var options = new FindOneAndUpdateOptions<UserRepresentation>
    {
      IsUpsert = true,
      ReturnDocument = ReturnDocument.After
    };
    var userUpdate = await users.FindOneAndUpdateAsync(filter, update, options);
    return userUpdate;
  }
  internal async Task<bool> IsPossibleToMute(long initiatorID, long targetID, ObjectId sirenaId)
  {
    var filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.Id, sirenaId)
    & (filterBuilder.Eq(x => x.OwnerId, targetID) | filterBuilder.AnyEq(x => x.Responsible, targetID))
    & filterBuilder.AnyEq(x => x.Listener, initiatorID);
    var anySirena = await sirens.Find(filter).AnyAsync();
    return anySirena;
  }

  internal async Task<List<long>> ValidateListeners(SirenRepresentation sirena, long initiatorID)
  {
    var muted = new UserRepresentation.MuteInfo(initiatorID, sirena.Id);
    var filterBuilder = Builders<UserRepresentation>.Filter;
    var filter = filterBuilder.In(x => x.UID, sirena.Listener)
              & !filterBuilder.AnyEq(x => x.Muted, muted);
    var projection = Builders<UserRepresentation>.Projection.Expression(x => x.UID);
    var listenersList = await users.Find(filter).Project(projection).ToListAsync();
    if (!sirena.OwnerId.Equals(initiatorID))
      listenersList.Add(sirena.OwnerId);
    return listenersList;
  }

  internal async Task<UserRepresentation> UnmuteUser(long initiatorID, long mutedUserID, ObjectId sirenaID)
  {
    var muted = new UserRepresentation.MuteInfo(mutedUserID, sirenaID);
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, initiatorID);
    var update = Builders<UserRepresentation>.Update.PullFilter(x => x.Muted, _muted => _muted.UID == mutedUserID);
    var options = new FindOneAndUpdateOptions<UserRepresentation>
    {
      ReturnDocument = ReturnDocument.After
    };
    var userUpdate = await users.FindOneAndUpdateAsync(filter, update);
    return userUpdate;
  }

  internal async Task<UpdateResult> RequestRightsForSirena(ObjectId sirenaId, long requesterId, string message)
  {
    var request = new SirenRepresentation.Request(requesterId, message);
    FilterDefinitionBuilder<SirenRepresentation> filterBuilder = Builders<SirenRepresentation>.Filter;
    var filter = filterBuilder.Eq(x => x.Id, sirenaId)
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
  public async Task<SirenRepresentation> DeleteUsersSirena(long uid, ObjectId id)
  {
    await DeleteSirenaIdFromOwner(uid, id);
    return await DeleteSirenaDocument(id);
  }
  public async Task<SirenRepresentation> DeleteSirenaDocument(ObjectId id)
  {
    var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    return await sirens.FindOneAndDeleteAsync(sirenFilter);
    // return await sirens.Find(sirenFilter).FirstOrDefaultAsync();
  }

  public async Task<UpdateResult> DeleteSirenaIdFromOwner(long uid, ObjectId id)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserRepresentation>.Update.Pull<ObjectId>(x => x.Owner, id);
    return await users.UpdateOneAsync(filter, userUpdate);
  }
}