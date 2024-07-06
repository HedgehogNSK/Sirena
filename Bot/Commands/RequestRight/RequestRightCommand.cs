using Hedgey.Extensions;
using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class RequestRightsCommand : AbstractBotCommmand
{
  public const string NAME = "request";
  public const string DESCRIPTION = "Request owner of sirena to delegate right to call sirena";
  private readonly FacadeMongoDBRequests requests;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;

  public RequestRightsCommand(FacadeMongoDBRequests requests
  , ILocalizationProvider localizationProvider
  , IMessageSender messageSender)
   : base(NAME, DESCRIPTION)
  {
    this.requests = requests;
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }

  public override async void Execute(IRequestContext context)
  {
    string responseText;
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    var param = context.GetArgsString().GetParameterByNumber(0);

    if (!ObjectId.TryParse(param, out ObjectId sid))
    {
      string wrongParameter = localizationProvider.Get("command.request_rights.incorrect_parameters", info);
      messageSender.Send(chatId, wrongParameter);
      return;
    }
    var userMessage = context.GetArgsString().SkipFirstNWords(1).BuildString();
    var updateResult = await requests.RequestRightsForSirena(sid, uid, userMessage);
    if (updateResult.MatchedCount == 0)
    {
      string failMessage = localizationProvider.Get("command.request_rights.fail", info);
      messageSender.Send(chatId, failMessage);
      return;
    }
    if (updateResult.ModifiedCount == 0)
    {
      string noChangesMessage = localizationProvider.Get("command.request_rights.already_sent", info);
      messageSender.Send(chatId, noChangesMessage);
      return;
    }
    string successMessage = localizationProvider.Get("command.request_rights.success", info);
    responseText = string.Format(successMessage, sid);
    messageSender.Send(chatId, responseText);
  }
  
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<RequestRightsCommand>(container)
  { }
}