using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class DelegateRightsCommand : AbstractBotCommmand
{
  public const string NAME = "delegate";
  public const string DESCRIPTION = "Delegate rights to call sirena to another user.";
  private readonly FacadeMongoDBRequests requests;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;
  public DelegateRightsCommand(FacadeMongoDBRequests requests
  , IMessageSender messageSender, ILocalizationProvider localizationProvider)
  : base(NAME, DESCRIPTION)
  {
    this.requests = requests;
    this.messageSender = messageSender;
    this.localizationProvider = localizationProvider;
  }

  public async override void Execute(IRequestContext context)
  {
    string responseText;

    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    string[] parameters = context.GetArgsString().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
    if (parameters.Length < 3)
    {
      string errorWrongParamters = localizationProvider.Get("command.delegate.error.incorrect_paramters", info);
      messageSender.Send(chatId, errorWrongParamters);
      return;
    }
    ObjectId sirenaId = default;
    if (!int.TryParse(parameters[1], out int number)
        && !ObjectId.TryParse(parameters[1], out sirenaId))
    {
      string errorWrongSirenaID = localizationProvider.Get("command.delegate.error.incorrect_id", info);
      responseText = string.Format(errorWrongSirenaID, parameters[1]);
      messageSender.Send(chatId, responseText);
      return;
    }
    if (!long.TryParse(parameters[2], out long duid))
    {
      string errorWrongUID = localizationProvider.Get("command.delegate.error.incorrect_user_id", info);
      responseText = string.Format(errorWrongUID, parameters[2]);
      messageSender.Send(chatId, responseText);
      return;
    }
    if (sirenaId == default)
    {
      //Get id of siren
      var sirena = await requests.GetSirenaBySerialNumber(uid, number);
      if (sirena == null)
        return;

      sirenaId = sirena.Id;
    }

    //Set responsible
    SirenRepresentation updatedSiren = await requests.SetUserResponsible(uid, sirenaId, duid);

    if (updatedSiren == null)
    {
      string errorNoSirena = localizationProvider.Get("command.delegate.error.no_sirena", info);
      responseText = string.Format(errorNoSirena, sirenaId);
      messageSender.Send(chatId, responseText);
      return;
    }
    string successMessage = localizationProvider.Get("command.delegate.success", info);
    responseText = string.Format(successMessage, duid, updatedSiren);
    messageSender.Send(chatId, responseText);
  }

  
}