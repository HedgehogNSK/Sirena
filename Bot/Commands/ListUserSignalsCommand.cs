using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;

namespace Hedgey.Sirena.Bot;
public class ListUserSignalsCommand : AbstractBotCommmand, IBotCommand
{
  public const string NAME = "list";
  public const string DESCRIPTION = "Shows a list of sirenas that are being tracked.";
  private IMongoCollection<SirenRepresentation> sirens;

  public ListUserSignalsCommand(IMongoDatabase db)
  : base(NAME, DESCRIPTION)
  {
    sirens = db.GetCollection<SirenRepresentation>("sirens");
  }

  public async override void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;

    var filter = Builders<SirenRepresentation>.Filter.Eq(x => x.OwnerId, uid);
    try
    {
      var userSirens = await sirens.Find(filter).ToListAsync();
      string messageText = CreateMessageText(userSirens);
      Program.messageSender.Send(uid, messageText);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
    }
  }

  private static string CreateMessageText(List<SirenRepresentation> userSirens)
  {
    StringBuilder builder = new StringBuilder();
    if (userSirens.Count != 0)
    {
      const string template = ". `{0}` *{1}*\n";

      int number = 0;
      builder.Append("The list of your sirens:\n");
      foreach (var siren in userSirens)
      {
        ++number;
        builder.Append(number).AppendFormat(template, siren.Id, siren.Title);
      }
    }
    else
    {
      const string noCreatedSirens = "You haven't created any siren yet.";
      builder.Append(noCreatedSirens);
    }
    var messageText = builder.ToString();
    return messageText;
  }
}