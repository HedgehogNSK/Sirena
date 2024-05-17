using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
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
    User botUser = context.GetUser();
    long uid = botUser.Id;
    long chatId = context.GetTargetChatId();

    IDisposable displaySirenasStream = getUserSirenas.GetUserSirenas(uid)
     .Subscribe(x => DisplaySirenas(x, chatId, uid));
  }

  private void DisplaySirenas(IEnumerable<SirenRepresentation> userSirenas, long chatId, long uid)
  {
    var enumerator = userSirenas.GetEnumerator();
    SirenRepresentation? sirena = null;
    if (enumerator.MoveNext())
      sirena = enumerator.Current;
    MessageBuilder message = sirena == null || enumerator.MoveNext() ?
      new UserSirenasMessageBuilder(chatId, userSirenas)
      : new SirenaInfoMessageBuilder(chatId, uid, sirena);
    messageSender.Send(message.Build());
  }
}