using Hedgey.Extensions;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class RequestRightsCommand : AbstractBotCommmand
{
  const string NAME = "request";
  const string DESCRIPTION = "Request owner of sirena to delegate right to call sirena";
  private const string noSirenaMessage = "Request failed. Possible reasons: \n1.There is no *sirena* with this id: *{0}*;\n2.You are *owner* of the sirena;\n3. You are already responsible for the sirena.";
  private const string noChangesMessage = "You have *already sent* a permission request for the sirena.";
  private const string successMessage = "You have successfuly sent a a permission request for the sirena: {0}";
  private readonly FacadeMongoDBRequests requests;
  private const string wrongParameter = "Incorrect parameters. Please use syntax:\n /request {sirena id} [request message]\nYou can send request message or skip it. But you have to set correc sirena id";

  public RequestRightsCommand(FacadeMongoDBRequests requests) : base(NAME, DESCRIPTION)
  {
    this.requests = requests;
  }

  public override async void Execute(ICommandContext context)
  {
    string responseText;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    var param = context.GetArgsString().GetParameterByNumber(0);

    if (!ObjectId.TryParse(param, out ObjectId sid))
    {
      Program.messageSender.Send(chatId, wrongParameter);
      return;
    }
    var userMessage = context.GetArgsString().SkipFirstNWords(1).BuildString();
    var updateResult = await requests.RequestRightsForSirena(sid, uid, userMessage);
    if (updateResult.MatchedCount == 0)
    {
      Program.messageSender.Send(chatId, noSirenaMessage);
      return;
    }
    if (updateResult.ModifiedCount == 0)
    {
      Program.messageSender.Send(chatId, noChangesMessage);
      return;
    }
    responseText = string.Format(successMessage,sid);
    Program.messageSender.Send(chatId, responseText);
  }
}