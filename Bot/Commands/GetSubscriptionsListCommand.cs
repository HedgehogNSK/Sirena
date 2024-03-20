using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;

namespace Hedgey.Sirena.Bot;
public class GetSubscriptionsListCommand : BotCustomCommmand, IBotCommand
{
  private IMongoCollection<UserRepresentation> usersCollection;
  private IMongoCollection<SirenRepresentation> sirenCollection;

  public GetSubscriptionsListCommand(string name, string description, IMongoDatabase db)
  : base(name, description)
  {
    usersCollection = db.GetCollection<UserRepresentation>("users");
    sirenCollection = db.GetCollection<SirenRepresentation>("sirens");
  }

  public async override void Execute(Message message)
  {
    long uid = message.From.Id;
    var filterBuilder = Builders<SirenRepresentation>.Filter;
    var currentUserFilter = filterBuilder.Exists(x => x.Listener)
    & filterBuilder.AnyEq(x=> x.Listener,uid);
    var subscriptionList =  await sirenCollection.Find(currentUserFilter).ToListAsync() ;
    if (!subscriptionList.Any())
    {
      string emptyListMessage = "You don't have any subscriptions";
      Program.messageSender.Send(message.Chat.Id, emptyListMessage);
      return;
    }

    string messageText = CreateMessageText(subscriptionList);
    Program.messageSender.Send(message.Chat.Id, messageText);
  }

  private static string CreateMessageText(List<SirenRepresentation> userSirens)
  {
    StringBuilder builder = new StringBuilder();
    if (userSirens.Count != 0)
    {
      int number = 0;
      builder.Append("Your subscriptions:\n");
      foreach (var siren in userSirens)
      {
        ++number;
        builder.Append(number).Append(' ').Append(siren.Id)
        .Append(' ')
        .Append('*')
        .Append(siren.Title)
        .Append('*')
        .Append('\n');
      }
    }
    else
    {
      builder.Append("You didn't subscribed on any sirenas yet.");
    }
    var messageText = builder.ToString();
    return messageText;
  }
}