using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;

public class StartCommand : AbstractBotCommmand
{
  public const string NAME = "start";
  public const string DESCRIPTION = "User initialization";
  private readonly FacadeMongoDBRequests requests;

  public StartCommand( FacadeMongoDBRequests requests)
     : base(NAME, DESCRIPTION)
  {
    this.requests = requests;
  }
  public override async void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    var user = await requests.CreateUser(uid, chatId);
    var message = new MenuMessageBuilder(uid, info, Program.LocalizationProvider).Build();
    message.Text = Program.LocalizationProvider.Get("command.start.welcome",info);
    Program.botProxyRequests.Send(message);
  }
  
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<StartCommand>(container)
  { }
}