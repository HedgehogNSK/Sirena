using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;
public class DisplayUsersSirenasCommand : AbstractBotCommmand, IBotCommand
{
  public const string NAME = "list";
  public const string DESCRIPTION = "Shows a list of sirenas that are being tracked.";
  private IGetUserRelatedSirenas getUserSirenas;
  private readonly IMessageSender messageSender;

  public DisplayUsersSirenasCommand(IGetUserRelatedSirenas getUserSirenas,
  IMessageSender messageSender)
  : base(NAME, DESCRIPTION)
  {
    this.getUserSirenas = getUserSirenas;
    this.messageSender = messageSender;
  }

  public override void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;

    IDisposable displaySirenasStream = getUserSirenas.GetUserSirenas(uid)
     .Subscribe(x => DisplaySirenas(x, context));
  }

  private void DisplaySirenas(IEnumerable<SirenRepresentation> userSirenas, IRequestContext context)
  {
    var enumerator = userSirenas.GetEnumerator();
    SirenRepresentation? sirena = null;
    if (enumerator.MoveNext())
      sirena = enumerator.Current;

    var info = context.GetCultureInfo();
    long uid = context.GetUser().Id;
    long chatId = context.GetTargetChatId();

    MessageBuilder message = sirena == null || enumerator.MoveNext() ?
      new UserSirenasMessageBuilder(chatId, info, Program.LocalizationProvider, userSirenas)
      : new SirenaInfoMessageBuilder(chatId, info, Program.LocalizationProvider, uid, sirena);
    messageSender.Send(message.Build());
  }
  public class Installer(SimpleInjector.Container container)
   : CommandInstaller<DisplayUsersSirenasCommand>(container)
  { }
}