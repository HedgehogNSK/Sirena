using System.Text;
using Hedgey.Extensions.Telegram;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class SearchSirenaCommand : AbstractBotCommmand
{
  const string NAME ="search" ;
  const string DESCRIPTION= "Find sirena by key string";
  const string errorWrongParamters = "Command usage: `/search {title}`\n At least 3 symbols ";
  private const string noSirenaError = "There is no sirena with title that contains search phrase: \"{0}\"";
  private const int MIN_SIMBOLS = 3;
  private const int MAX_SIMBOLS = 200;
  private FacadeMongoDBRequests requests;
  private readonly TelegramBot bot;

  public SearchSirenaCommand(FacadeMongoDBRequests requests, TelegramBot bot)
: base(NAME, DESCRIPTION)
  {
    this.requests = requests;
    this.bot = bot;
  }

  public override async void Execute(ICommandContext context)
  {
    string responseText;
    long chatId = context.GetChat().Id;
    string parameters = context.GetArgsString();
    if (parameters.Length < MIN_SIMBOLS || parameters.Length > MAX_SIMBOLS)
    {
      Program.messageSender.Send(chatId, errorWrongParamters);
      return;
    }
    var searchKey = parameters;
    var sirenasList = await requests.GetSirenaByName(searchKey);
    if (!sirenasList.Any())
    {
      responseText = string.Format(noSirenaError, searchKey);
      Program.messageSender.Send(chatId, responseText);
      return;
    }

    StringBuilder builder = new StringBuilder("Found sirenas:\n");
    int number = 1;
    foreach (var sirena in sirenasList)
    {
      var owner = await BotTools.GetUsername(bot, sirena.OwnerId);
      builder.Append(number)
      .Append('.').Append(' ')
      .Append('_').Append(sirena.Id).Append('_')
      .Append(' ')
      .Append(owner)
      .Append(' ').AppendLine()
      .Append('*').Append(sirena.Title).Append('*')
      .AppendLine().AppendLine();

      ++number;
    }
    
    InlineKeyboardButton[] array =
            [
                new InlineKeyboardButton { Text = "Subscribe", CallbackData = "button1" },
                new InlineKeyboardButton { Text = "Request", CallbackData = "button2" }
            ];
    var keyboard = new InlineKeyboardMarkup
    {
      InlineKeyboard = [array]
    };
    Program.messageSender.Send(chatId, builder.ToString(),keyboard);
  }
}