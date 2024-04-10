using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class SubscribeCommand : AbstractBotCommmand
{
  const string NAME ="subscribe" ;
  const string DESCRIPTION = "Subscribes to *sirena* by id.";
  const string noSirenaError = "There is no *sirena* with this id: *{0}*";
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  public SubscribeCommand( IMongoDatabase db)
  : base(NAME, DESCRIPTION)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
  }

  public async override void Execute(ICommandContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string param = context.GetArgsString().GetParameterByNumber(0);
    string notificationText;
    if (string.IsNullOrEmpty(param))
    {
      notificationText = $"You have to input *Id* of a siren to subsribe";
      Program.messageSender.Send(chatId, notificationText);
      return;
    }

    if (!ObjectId.TryParse(param, out ObjectId id))
    {
      notificationText = string.Format(noSirenaError, param);
      Program.messageSender.Send(chatId, notificationText);
      return;
    }

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, uid);
    var siren = await sirenCollection.FindOneAndUpdateAsync(filterSiren, addSubsription);
    if (siren == null)
    {
      notificationText = string.Format(noSirenaError, param);
      Program.messageSender.Send(chatId, notificationText);
      return;
    }
    notificationText = $"You successfully subsribed to *siren*: _{siren.Title}_";
    Program.messageSender.Send(chatId, notificationText);
  }
}