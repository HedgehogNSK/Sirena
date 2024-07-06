using Hedgey.Sirena.Database;
using MongoDB.Driver;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class GetUserOperationAsync : IGetUserOperationAsync
{
  private IMongoCollection<UserRepresentation> usersCollection;
  private readonly FacadeMongoDBRequests requests;
  private readonly IMessageSender messageSender;

  public GetUserOperationAsync(IMongoCollection<UserRepresentation> usersCollection
  , FacadeMongoDBRequests requests, IMessageSender messageSender)
  {
    this.usersCollection = usersCollection;
    this.requests = requests;
    this.messageSender = messageSender;
  }
  public async Task<UserRepresentation?> GetAsync(long uid)
  {
    var filter = Builders<UserRepresentation>.Filter.Eq("_id", uid);
    var user = await usersCollection.Find(filter).FirstOrDefaultAsync();
    if (user == null)
    {
      user = await requests.CreateUser(uid, uid);
      if(user==null)
        messageSender.Send(uid, "Database couldn't create user. Please try latter");
      }
      return user;
  }
}