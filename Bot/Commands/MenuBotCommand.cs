using Hedgey.Sirena.Bot.Operations;

namespace Hedgey.Sirena.Bot;

public class MenuBotCommand : AbstractBotCommmand
{
  const string NAME = "menu";
  const string DESCRIPTION = "Displays primary bot functions";
  private readonly IGetUserOverviewAsync getOverview;

  public MenuBotCommand(IGetUserOverviewAsync getOverview) : base(NAME, DESCRIPTION)
  {
    this.getOverview = getOverview;
  }

  public override async void Execute(IRequestContext context)
  {
    var uid = context.GetUser().Id;
    var result = await getOverview.Get(uid);
    var messageBuilder = new MenuMessageBuilder(uid).AddUserStatistics(result);
    var message = messageBuilder.Build();
    Program.messageSender.Send(message);
    Console.WriteLine(result);
  }
}
