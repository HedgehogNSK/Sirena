using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;

namespace Hedgey.Sirena.Bot;

public class MenuBotCommand : AbstractBotCommmand
{
  public const string NAME = "menu";
  public const string DESCRIPTION = "Displays primary bot functions";
  private readonly IGetUserOverviewAsync getOverview;
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;

  public MenuBotCommand(IGetUserOverviewAsync getOverview
  , ILocalizationProvider localizationProvider, IMessageSender messageSender)
  : base(NAME, DESCRIPTION)
  {
    this.getOverview = getOverview;
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }

  public override async void Execute(IRequestContext context)
  {
    var info = context.GetCultureInfo();
    var uid = context.GetUser().Id;
    var result = await getOverview.Get(uid);
    var messageBuilder = new MenuMessageBuilder(uid, info, localizationProvider).AddUserStatistics(result);
    var message = messageBuilder.Build();
    messageSender.Send(message);
  }
}