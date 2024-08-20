using Hedgey.Localization;

namespace Hedgey.Sirena.Bot;

public class StartCommand : AbstractBotCommmand
{
  public const string NAME = "start";
  public const string DESCRIPTION = "User initialization";
  private readonly ILocalizationProvider localizationProvider;
  private readonly IMessageSender messageSender;

  public StartCommand(ILocalizationProvider localizationProvider, IMessageSender messageSender)
     : base(NAME, DESCRIPTION)
  {
    this.localizationProvider = localizationProvider;
    this.messageSender = messageSender;
  }
  public override void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;
    var info = context.GetCultureInfo();
    var message = new MenuMessageBuilder(uid, info, localizationProvider).Build();
    message.Text = localizationProvider.Get("command.start.welcome",info);
    messageSender.Send(message);
  }
}