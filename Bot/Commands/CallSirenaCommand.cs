using Hedgey.Extensions;
using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;
public class CallSirenaCommand : AbstractBotCommmand
{
  public const string NAME = "call";
  public const string DESCRIPTION = "Call sirena by number or by id";
  const string errorWrongParamters = "Please input: /call {siren number or id}";
  const string errorWrongSirenaID = "{0} parameter is incorrect. First parameter has to be serial number or ID of your sirena";
  const string errorNoSirena = "You don't have a sirena with id: {0}";
  const string errorNoSubscribers = "The sirena: {0} doesn't have any recievers";
  const string errorRecentlyCall = "This sirena has been called recently. You have to wait at least 1 minute before call sirena again.\n {0} time: {1}";
  const string successMessage = "Calling {0} subscribers.";
  const string notificationBase = "*{0}*\n _Called at {3} by {1}|{2}._";
  private const int MESSAGE_MAX_SYMBOLS = 150;
  private static TimeSpan delay = TimeSpan.FromMinutes(1);
  private readonly FacadeMongoDBRequests requests;

  public CallSirenaCommand(FacadeMongoDBRequests requests)
     : base(NAME, DESCRIPTION)
  {
    this.requests = requests;
  }

  public async override void Execute(IRequestContext context)
  {
    string responseText;

    User user = context.GetUser();
    long uid = user.Id;
    long chatid = context.GetChat().Id;

    string[] parameters = context.GetArgsString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length == 0)
    {
      Program.messageSender.Send(chatid, errorWrongParamters);
      return;
    }
    ObjectId sirenaId = default;
    string sirenaParam = parameters[0];
    if (!int.TryParse(sirenaParam, out int number)
        && !ObjectId.TryParse(sirenaParam, out sirenaId))
    {
      responseText = string.Format(errorWrongSirenaID, sirenaParam);
      Program.messageSender.Send(chatid, responseText);
      return;
    }
    SirenRepresentation? sirena;
    if (sirenaId == default)
    {
      //Get id of siren
      sirena = await requests.GetSirenaBySerialNumber(uid, number);
    }
    else
    {
      sirena = await requests.GetSirenaById(sirenaId);
    }
    if (sirena == null)
    {
      responseText = string.Format(errorNoSirena, parameters[0]);
      Program.messageSender.Send(chatid, responseText);
      return;
    }

    var now = DateTimeOffset.Now;
    const string dateFormat = "HH:mm:ss dd.MM.yyyy";
    var dateString = now.ToString(dateFormat);

    if (sirena.LastCall != null && sirena.LastCall.Date + delay > now)
    {
      responseText = string.Format(errorRecentlyCall, sirena, sirena.LastCall.Date.ToString(dateFormat));
      Program.messageSender.Send(chatid, responseText);
      return;
    }
    var recievers = await requests.ValidateListeners(sirena, uid);
    if (recievers.Count == 0)
    {
      responseText = string.Format(errorNoSubscribers, sirena);
      Program.messageSender.Send(chatid, responseText);
      return;
    }

    var username = BotTools.GetUsername(user);
    string notification = string.Format(notificationBase, sirena.Title, username, uid, dateString);
    if (parameters.Length == 2)
    {
      var userAdditiveText = parameters[1].Take(MESSAGE_MAX_SYMBOLS).BuildString();
      notification = '*' + userAdditiveText + '*' + '\n' + notification;
    }
    responseText = string.Format(successMessage, recievers.Count);

    Program.messageSender.Send(chatid, responseText);

    SirenRepresentation.CallInfo lastCall = new SirenRepresentation.CallInfo(uid, now);
    var result = requests.SetCallDate(sirena.Id, lastCall);
    foreach (var listener in recievers)
      Program.messageSender.Send(listener, notification, null, false);
  }
}