using Hedgey.Sirena.Database;
using MongoDB.Driver;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class GetUserOperationAsync : IGetUserOperationAsync
{
  private IMongoCollection<UserRepresentation> users;
  private readonly FacadeMongoDBRequests requests;
  private readonly IMessageSender messageSender;

  public GetUserOperationAsync(IMongoCollection<UserRepresentation> usersCollection
  , FacadeMongoDBRequests requests, IMessageSender messageSender)
  {
    this.users = usersCollection;
    this.requests = requests;
    this.messageSender = messageSender;
  }
  public async Task<UserRepresentation?> GetAsync(long uid)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq("_id", uid);
    try
    {
      var user = await users.Find(filter).FirstOrDefaultAsync();

      if (user == null)
      {
        user = await requests.CreateUser(uid, uid);
        if (user == null)
          messageSender.Send(uid, "Database couldn't create user. Please try latter");
      }
      return user;
    }
    catch (Exception ex)
    {
      throw new Exception($"Exception on find user with id {uid}", ex);
    }
  }
}