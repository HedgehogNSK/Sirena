using MongoDB.Bson;
using MongoDB.Driver;

namespace Hedgey.Sirena.Database;

public class FacadeMongoDBRequests
{
  private readonly MongoClient client;
  private readonly IMongoDatabase db;
  private readonly IMongoCollection<SirenRepresentation> sirens;
  private readonly IMongoCollection<UserRepresentation> users;

  public FacadeMongoDBRequests(MongoClient client)
  {
    this.client = client;
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
          Program.messageSender.Send(uid, failMessageText);
        }
      }
    return id;
  }
  public async Task<UserRepresentation> CreateUser(long userID, long chatID)
  {
    Database.UserRepresentation user = new Database.UserRepresentation
    {
      UID = userID,
      ChatID = chatID,
      Owner = [],
    };
    await users.InsertOneAsync(user);
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
        .AddToSet(x => x.Responsible, uid)
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

  internal async Task<SirenRepresentation>  SetCallDate(ObjectId id, DateTimeOffset callTime)
  {
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var update = Builders<SirenRepresentation>.Update.Set(x=> x.LastCall, callTime);
    var sirena = await sirens.FindOneAndUpdateAsync(filter,update);
    return sirena;
  }
}