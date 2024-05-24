using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class StartCommand : AbstractBotCommmand
{
  public const string NAME = "start";
  public const string DESCRIPTION = "User initialization";
  const string welcomeMessage = "Welcome to *Sirena bot*!\nThis bot proivdes a mechanism for quick notifications. You can create notifications (*Sirena*). People subscribes to your notification. When time comes just call the Sirena and all of the subscribers will get your message.\n\nYou can use *Menu* (/menu) to manage the bot. You can call commands directly either. To find out full list of the commands please use /help command.";
  private readonly FacadeMongoDBRequests requests;

  public StartCommand( FacadeMongoDBRequests requests)
     : base(NAME, DESCRIPTION)
  {
    this.requests = requests;
  }
  public override async void Execute(IRequestContext context)
  {
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetChat().Id;
    var user = await requests.CreateUser(uid, chatId);
    var message = new MenuMessageBuilder(uid).Build();
    message.Text = welcomeMessage;
    Program.botProxyRequests.Send(message);
  }
}