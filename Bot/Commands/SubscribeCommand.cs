using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class SubscribeCommand : BotCustomCommmand
{
  const string noSirenaError = "There is no *sirena* with this id: *{0}*";
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  public SubscribeCommand(string name, string description, IMongoDatabase db)
  : base(name, description)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
  }

  public async override void Execute(Message message)
  {
    long uid = message.From.Id;
    string param = message.Text.GetParameterByNumber(1);
    string notificationText;
    if (string.IsNullOrEmpty(param))
    {
      notificationText = $"You have to input *Id* of a siren to subsribe";
      Program.messageSender.Send(message.Chat.Id, notificationText);
      return;
    }

    if (!ObjectId.TryParse(param, out ObjectId id))
    {
      notificationText = string.Format(noSirenaError, param);
      Program.messageSender.Send(message.Chat.Id, notificationText);
      return;
    }

    var filterSiren = Builders<SirenRepresentation>.Filter.Eq(x => x.Id, id);
    var addSubsription = Builders<SirenRepresentation>.Update.AddToSet(x => x.Listener, uid);
    var siren = await sirenCollection.FindOneAndUpdateAsync(filterSiren, addSubsription);
    if (siren == null)
    {
      notificationText = string.Format(noSirenaError, param);
      Program.messageSender.Send(message.Chat.Id, notificationText);
      return;
    }
    notificationText = $"You successfully subsribed to *siren*: _{siren.Title}_";
    Program.messageSender.Send(message.Chat.Id, notificationText);
  }
}