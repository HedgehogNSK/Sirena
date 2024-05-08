using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class RemoveSirenCommand : AbstractBotCommmand
{
  const string NAME ="remove" ;
  const string DESCRIPTION = "Remove your sirena by number, or by id.";
  private const string wrongParameter = "Command syntax: `/remove {sirena number| sirena number}`\n You can find sirena id and number using /list";
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  private FacadeMongoDBRequests request;

  public RemoveSirenCommand( IMongoDatabase db, FacadeMongoDBRequests request)
  : base(NAME, DESCRIPTION)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
    this.request = request;
  }
  public record IdProjection(ObjectId? Id);
  async public override void Execute(IRequestContext context)
  {
     string messageText ;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string param = context.GetArgsString().GetParameterByNumber(0);
    ObjectId id = await request.GetSirenaId(uid, param);
    if (id == ObjectId.Empty)
    {
      Program.messageSender.Send(chatId, wrongParameter);
      return;
    }
    //Remove srien Id from the owner document
    var filter = Builders<UserRepresentation>.Filter.Eq(x => x.UID, uid);
    var userUpdate = Builders<UserRepresentation>.Update.Pull<ObjectId>(x => x.Owner, id);
    var userUpdateResult = await usersCollection.UpdateOneAsync(filter, userUpdate);

    //Remove siren from collection by ID
    var sirenFilter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var result2 = await sirenCollection.FindOneAndDeleteAsync(sirenFilter);
    messageText = result2 != null ? '*' + result2.Title + "* has been removed" :
    "You don't have *sirena* with id: *" + id + '*';
    Program.messageSender.Send(chatId, messageText);
  }
}