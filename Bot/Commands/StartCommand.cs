using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class StartCommand : AbstractBotCommmand
{
  public const string NAME = "start";
  public const string DESCRIPTION = "Initialization of user";
  const string welcomeMessage = "Welcome to *Sirena bot*. This bot proivdes mechanism for notification people.\nPlease start from /help command to discover available functions of the Sirena bot";
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
    Program.messageSender.Send(message);
  }
}