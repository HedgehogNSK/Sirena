using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;

namespace Hedgey.Sirena.Bot;

public class MenuBotCommand : AbstractBotCommmand
{
  public const string NAME = "menu";
  public const string DESCRIPTION = "Displays primary bot functions";
  private readonly IGetUserOverviewAsync getOverview;
  private readonly ILocalizationProvider localizationProvider;

  public MenuBotCommand(IGetUserOverviewAsync getOverview, ILocalizationProvider localizationProvider) : base(NAME, DESCRIPTION)
  {
    this.getOverview = getOverview;
    this.localizationProvider = localizationProvider;
  }

  public override async void Execute(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    var uid = context.GetUser().Id;
    var result = await getOverview.Get(uid);
    var messageBuilder = new MenuMessageBuilder(uid, info, localizationProvider).AddUserStatistics(result);
    var message = messageBuilder.Build();
    Program.botProxyRequests.Send(message);
  }
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<MenuBotCommand>(container)
  { }
}
