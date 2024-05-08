using Hedgey.Sirena.Database;
using MongoDB.Driver;

namespace Hedgey.Sirena.Bot.Operations.Mongo;

public class CreateSirenaOperationAsync : ICreateSirenaOperationAsync
{
  private readonly IMongoCollection<SirenRepresentation> sirenCollection;
  private readonly IMongoCollection<UserRepresentation> usersCollection;

  public CreateSirenaOperationAsync(IMongoCollection<SirenRepresentation> sirenCollection
  ,IMongoCollection<UserRepresentation> usersCollection)
  {
    this.sirenCollection = sirenCollection;
    this.usersCollection = usersCollection;
  }
  public async Task<SirenRepresentation> CreateAsync(long uid, string sirenName)
  {
    SirenRepresentation siren = new SirenRepresentation
    {
      Title = sirenName,
      OwnerId = uid,
      UseCount = 0
    };
    await sirenCollection.InsertOneAsync(siren);

    var update = Builders<UserRepresentation>.Update
    .AddToSet(_user => _user.Owner, siren.Id);
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