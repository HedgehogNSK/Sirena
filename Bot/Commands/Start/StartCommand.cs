using Hedgey.Localization;
using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;

public class StartCommand : AbstractBotCommmand
{
  public const string NAME = "start";
  public const string DESCRIPTION = "User initialization";
  private readonly FacadeMongoDBRequests requests;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;

  public StartCommand(ILocalizationProvider localizationProvider, IMessageSender messageSender)
     : base(NAME, DESCRIPTION)
  {
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }
  public override async void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;
    long chatId = context.GetChat().Id;
    var info = context.GetCultureInfo();
    var user = await requests.CreateUser(uid, chatId);
    var message = new MenuMessageBuilder(uid, info, localizationProvider).Build();
    message.Text = localizationProvider.Get("command.start.welcome",info);
    messageSender.Send(message);
  }
  
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<StartCommand>(container)
  { }
}