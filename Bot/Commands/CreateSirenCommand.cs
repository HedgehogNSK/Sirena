using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaCommand : AbstractBotCommmand
{
  const string NAME ="create";
  const string DESCRIPTION = "Creates a sirena with certain title. Example: `/create Sirena`";
  private const int SIGNAL_LIMIT = 5;
  private const int TITLE_MAX_LENGHT = 256;
  private const int TITLE_MIN_LENGHT = 3;
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  private readonly FacadeMongoDBRequests requests;
  private const string emptyTitleWarning = "Command syntax: `/create {{title}}`\nSirena title must be between {0} and {1} symbols long.";

  public CreateSirenaCommand(IMongoDatabase db, FacadeMongoDBRequests requests)
  : base(NAME, DESCRIPTION)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
    this.requests = requests;
  }

  async public override void Execute(ICommandContext context)
  {    
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    string sirenName = context.GetArgsString();
    if (string.IsNullOrEmpty(sirenName) || sirenName.Length < TITLE_MIN_LENGHT)
    {
      var text =string.Format(emptyTitleWarning, TITLE_MIN_LENGHT, TITLE_MAX_LENGHT);
      Program.messageSender.Send(chatId, text);
      return;
    }
    var filter = Builders<UserRepresentation>.Filter.Eq("_id", uid);
    var user = await usersCollection.Find(filter).FirstOrDefaultAsync();
    if (user == null)
    {
      user =await requests.CreateUser(uid, chatId);
      if(user==null)
        Program.messageSender.Send(chatId, "Database couldn't create user. Please try latter");
        return;
      }
    var ownedSignalsCount = user.Owner.Length;
    if (ownedSignalsCount >= SIGNAL_LIMIT)
    {
      var username = BotTools.GetUsername(botUser);

      Console.WriteLine($"{username} is already an owner of {ownedSignalsCount} signals");
      const string messagText = "You reached the limit of available signals number. Delete one of your current signals to create a new one.";
      Program.messageSender.Send(chatId, messagText);
      return;
    }
    SirenRepresentation siren = await CreateSiren(sirenName, user);

    var updatedArray = user.Owner.Append(siren.Id);
    var update = Builders<UserRepresentation>.Update
    .Set(_user => _user.Owner, updatedArray);

    await usersCollection.UpdateOneAsync<UserRepresentation>(
      filter: x => x.UID.Equals(user.UID),
      update: update);
    const string success = "Signal has been created successfuly. It's ID: *'{0}'*. Provide it to subscribers.";
    Program.messageSender.Send(chatId, string.Format(success, siren.Id));
  }

  private async Task<SirenRepresentation> CreateSiren(string sirenName, UserRepresentation user)
  {
    SirenRepresentation siren = new SirenRepresentation
    {
      Title = sirenName,
      OwnerId = user.UID,
      UseCount = 0
    };
    await sirenCollection.InsertOneAsync(siren);
    return siren;
  }
}