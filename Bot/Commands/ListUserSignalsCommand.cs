using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;

namespace Hedgey.Sirena.Bot;
public class ListUserSignalsCommand : BotCustomCommmand, IBotCommand
{
  private IMongoCollection<SirenRepresentation> sirens;

  public ListUserSignalsCommand(string name, string description, IMongoDatabase db)
  : base(name, description)
  {
    sirens = db.GetCollection<SirenRepresentation>("sirens");
  }

  public async override void Execute(Message message)
  {
    long uid = message.From.Id;

    var filter = Builders<SirenRepresentation>.Filter.Eq("ownerid", uid);
    var userSirens = await sirens.Find<SirenRepresentation>(filter).ToListAsync();
    string messageText = CreateMessageText(userSirens);
    Program.messageSender.Send(message.From.Id, messageText);
  }

  private static string CreateMessageText(List<SirenRepresentation> userSirens)
  {
    StringBuilder builder = new StringBuilder();
    if (userSirens.Count != 0)
    {
      int number = 0;
      builder.Append("The list of your sirens:\n");
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
      builder.Append("You haven't created any signals yet.");
    }
    var messageText = builder.ToString();
    return messageText;
  }
}