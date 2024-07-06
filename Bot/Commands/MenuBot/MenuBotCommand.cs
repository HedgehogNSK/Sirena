using Hedgey.Sirena.Bot.Operations;

namespace Hedgey.Sirena.Bot;

public class MenuBotCommand : AbstractBotCommmand
{
  public const string NAME = "menu";
  public const string DESCRIPTION = "Displays primary bot functions";
  private readonly IGetUserOverviewAsync getOverview;

  public MenuBotCommand(IGetUserOverviewAsync getOverview) : base(NAME, DESCRIPTION)
  {
    this.getOverview = getOverview;
  }

  public override async void Execute(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    var uid = context.GetUser().Id;
    var result = await getOverview.Get(uid);
    var messageBuilder = new MenuMessageBuilder(uid, info, Program.LocalizationProvider).AddUserStatistics(result);
    var message = messageBuilder.Build();
    Program.botProxyRequests.Send(message);
  }
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<MenuBotCommand>(container)
  { }
}
