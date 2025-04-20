using Hedgey.Extensions;
using Hedgey.Extensions.Telegram;
using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Blendflake;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Text;
using Hedgey.Telegram.Bot;
using Hedgey.Sirena.MongoDB;

namespace Hedgey.Sirena.Bot;

public class GetResponsiblesListCommand : AbstractBotCommmand
{
  public const string NAME = "responsible";
  public const string DESCRIPTION = "Display list of user allowed to call Sirena";
  private readonly TelegramBot bot;
  private readonly FacadeMongoDBRequests requests;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;

  public GetResponsiblesListCommand(FacadeMongoDBRequests requests
  , TelegramBot bot, ILocalizationProvider localizationProvider
  , IMessageSender messageSender)
  : base(NAME, DESCRIPTION)
  {
    this.bot = bot;
    this.requests = requests;
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }

  public override async void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    string param = context.GetArgsString().GetParameterByNumber(0);
    SirenaData sirena;

    if (int.TryParse(param, out int number))
    {
      var result = await requests.GetSirenaBySerialNumber(uid, number);
      if (result != null)
      {
        sirena = result;
      }
      else
      {
        string noSirenaWithNumber = localizationProvider.Get("command.get_responsibles.no_sirena_number", info);
        messageSender.Send(chatId, string.Format(noSirenaWithNumber, number));
        return;
      }
    }
    else if (HashUtilities.TryParse(param, out var id))
    {
      sirena = await requests.GetSirenaById(id);
      if (sirena == null)
      {

        string noSirenaMessage = localizationProvider.Get("command.get_responsibles.no_sirena_id", info);
        messageSender.Send(chatId, noSirenaMessage);
        return;
      }
    }
    else
    {
      string wrongParamMessage = localizationProvider.Get("command.get_responsibles.incorrect_parameters", info);
      messageSender.Send(chatId, wrongParamMessage);
      return;
    }

    var messageText = await CreateMessageText(sirena, info);
    messageSender.Send(chatId, messageText);
  }

  private async Task<string[]> GetResponsibleNames(SirenaData sirena)
  {
    string[] names = new string[sirena.Responsible.Length];
    for (int id = 0; id != sirena.Responsible.Length; ++id)
    {
      var uid = sirena.Responsible[id];
      string nick = await bot.GetDisplayName(uid);
      names[id] = $"{nick}|{uid}";
    }

    return names;
  }

  private async Task<string> CreateMessageText(SirenaData? sirena, System.Globalization.CultureInfo info)
  {
    if (sirena == null)
      return localizationProvider.Get("command.get_responsibles.no_sirena", info);

    string owner = await bot.GetDisplayName(sirena.OwnerId);
    owner += "|" + sirena.OwnerId;
    string template = localizationProvider.Get("command.get_responsibles.template", info);
    var builder = new StringBuilder().AppendFormat(template, sirena.Title, owner);

    if (sirena.Responsible.Any())
    {
      int number = 0;
      string[] responsibles = await GetResponsibleNames(sirena);

      foreach (var responsible in responsibles)
      {
        ++number;
        builder.Append(number).Append(". ").Append(responsible).AppendLine();
      }
    }
    return builder.ToString();
  }
}