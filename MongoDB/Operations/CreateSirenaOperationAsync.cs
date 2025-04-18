using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Entities;
using Hedgey.Sirena.ID;
using MongoDB.Driver;

namespace Hedgey.Sirena.MongoDB.Operations;

public class CreateSirenaOperationAsync : ICreateSirenaOperationAsync
{
  private readonly IMongoCollection<SirenaData> sirenCollection;
  private readonly IMongoCollection<UserData> usersCollection;
  private readonly IIDGenerator idGenerator;

  public CreateSirenaOperationAsync(IMongoCollection<SirenaData> sirenCollection
  ,IMongoCollection<UserData> usersCollection
  ,IIDGenerator idGenerator )
  {
    this.sirenCollection = sirenCollection;
    this.usersCollection = usersCollection;
    this.idGenerator = idGenerator;
  }
  public async Task<SirenaData> CreateAsync(long uid, string sirenName)
  {

    SirenaData siren = new SirenaData
    {
      Title = sirenName,
      SID = idGenerator.Get(),
      OwnerId = uid,
      UseCount = 0
    };
    await sirenCollection.InsertOneAsync(siren);

    var update = Builders<UserData>.Update
    .AddToSet(_user => _user.Owner, siren.SID);
    UpdateOptions updateOptions = new UpdateOptions()
    {
      IsUpsert = true,
    };
    await usersCollection.UpdateOneAsync(
      filter: x => x.UID.Equals(uid),
      update: update,
      options: updateOptions);
    return siren;
  }
}