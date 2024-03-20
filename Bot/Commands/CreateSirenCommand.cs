using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text.RegularExpressions;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaCommand : BotCustomCommmand
{
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;
  private readonly FacadeMongoDBRequests requests;
  private const int SIGNAL_LIMIT = 5;
  private const int TITLE_MAX_LENGHT = 256;
  private const int TITLE_MIN_LENGHT = 3;
  private const string emptyTitleWarning = "Signal title must have at least" +
  " {0} symbols and must not be more than {1} symbols long";

  public CreateSirenaCommand(string name, string description, IMongoDatabase db, FacadeMongoDBRequests requests)
  : base(name, description)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
    this.requests = requests;
  }

  async public override void Execute(Message message)
  {
    long uid = message.From.Id;
    long chatId = message.Chat.Id;
    var msgTxt =  Regex.Escape(message.Text);
    string sirenName = message.Text.SkipWhile(x => x != ' ').Skip(1)
      .Take(TITLE_MAX_LENGHT).ConvertToString();
    if (string.IsNullOrEmpty(sirenName) || sirenName.Length < TITLE_MIN_LENGHT)
    {
      Program.messageSender.Send(message.Chat.Id, string.Format(emptyTitleWarning, TITLE_MIN_LENGHT, TITLE_MAX_LENGHT));
      return;
    }
    var filter = Builders<UserRepresentation>.Filter.Eq("_id", uid);
    var user = await usersCollection.Find(filter).FirstOrDefaultAsync();
    if (user == null)
    {
      user =await requests.CreateUser(message.From.Id, message.Chat.Id);
      if(user==null)
        Program.messageSender.Send(message.Chat.Id, "Database couldn't create user. Please try latter");
        return;
      }
    var ownedSignalsCount = user.Owner.Length;
    if (ownedSignalsCount >= SIGNAL_LIMIT)
    {
      Console.WriteLine($"{message.From.Username} is already an owner of {ownedSignalsCount} signals");
      const string messagText = "You reached the limit of available signals number. Delete one of your current signals to create a new one.";
      Program.messageSender.Send(message.Chat.Id, messagText);
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
    Program.messageSender.Send(message.Chat.Id, string.Format(success, siren.Id));
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