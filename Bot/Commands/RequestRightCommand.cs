using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class RequestRightsCommand : BotCustomCommmand
{
  private const string noSirenaMessage = "Request failed. Possible reasons: \n1.There is no *sirena* with this id: *{0}*;\n2.You are *owner* of the sirena;\n3. You are already responsible for the sirena.";
  private const string noChangesMessage = "You have *already sent* a permission request for the sirena.";
  private const string successMessage = "You have successfuly sent a a permission request for the sirena: {0}";
  private readonly FacadeMongoDBRequests requests;
  private const string wrongParameter = "Incorrect parameters. Please use syntax:\n /request {sirena id} [request message]\nYou can send request message or skip it. But you have to set correc sirena id";

  public RequestRightsCommand(string name, string description
  , FacadeMongoDBRequests requests) : base(name, description)
  {
    this.requests = requests;
  }

  public override async void Execute(Message message)
  {
    string responseText;
    var uid = message.From.Id;
    var param = message.Text.GetParameterByNumber(1);

    if (!ObjectId.TryParse(param, out ObjectId sid))
    {
      Program.messageSender.Send(message.Chat.Id, wrongParameter);
      return;
    }
    var userMessage = message.Text.SkipFirstNWords(2);
    var updateResult = await requests.RequestRightsForSirena(sid, uid, userMessage);
    if (updateResult.MatchedCount == 0)
    {
      Program.messageSender.Send(message.Chat.Id, noSirenaMessage);
      return;
    }
    if (updateResult.ModifiedCount == 0)
    {
      Program.messageSender.Send(message.Chat.Id, noChangesMessage);
      return;
    }
    responseText = string.Format(successMessage,sid);
    Program.messageSender.Send(message.Chat.Id, responseText);
  }
}