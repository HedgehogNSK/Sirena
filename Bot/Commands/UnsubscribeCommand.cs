using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class UnsubscribeCommand : AbstractBotCommmand
{
  public const string NAME ="unsubscribe";
  public const string DESCRIPTION = "Unsubscribes from certain sirena.";

  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  public UnsubscribeCommand( IMongoDatabase db)
  : base(NAME, DESCRIPTION)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
  }

  public async override void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string param = context.GetArgsString().GetParameterByNumber(0);
    if (string.IsNullOrEmpty(param) || !ObjectId.TryParse(param, out ObjectId id))
    {
      string failMessageText = $"You have to input *valid Id* of existing sirena";
      Program.messageSender.Send(chatId, failMessageText);
      return;
    }

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq("_id", id);
    var siren = await sirenCollection.Find(filterSiren).FirstOrDefaultAsync();
    if (siren == null)
    {
      string failMessageText = $"There is now *sirena* with this id: *{param}*";
      Program.messageSender.Send(chatId, failMessageText);
      return;
    }
    //Remove uid from listeners of certain sirena array of the user document
    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var update = Builders<SirenRepresentation>.Update.Pull(x => x.Listener, uid);
    var result = await sirenCollection.UpdateOneAsync(filter, update);
    if (result != null)
    {

      string failMessageText = result.ModifiedCount != 0 ? $"You unsubsribed from *siren*  _{siren.Title}_ successfully." :
      $"You haven't been subsribed on this *siren* _{siren.Title}_ yet!";
      Program.messageSender.Send(chatId, failMessageText);
    }
  }
}