using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Reactive.Linq;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;
public sealed class DisplayUsersSirenasCommand(IGetUserRelatedSirenas getUserSirenas
  , IMessageSender messageSender
  , ILocalizationProvider localizationProvider)
  : AbstractBotCommmand(NAME, DESCRIPTION), IBotCommand, IDisposable
{
  public const string NAME = "list";
  public const string DESCRIPTION = "Shows a list of sirenas that are being tracked.";
  private readonly IGetUserRelatedSirenas getUserSirenas = getUserSirenas;
  private readonly IMessageSender messageSender = messageSender;
  private readonly ILocalizationProvider localizationProvider = localizationProvider;
  private IDisposable? displaySirenasStream;

  public void Dispose()
  {
    displaySirenasStream?.Dispose();
  }

  public override void Execute(IRequestContext context)
  {
    long uid = context.GetUser().Id;

    displaySirenasStream = getUserSirenas.GetUserSirenas(uid)
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
      new UserSirenasMessageBuilder(chatId, info, localizationProvider, userSirenas)
      : new SirenaInfoMessageBuilder(chatId, info, localizationProvider, uid, sirena);
    messageSender.Send(message.Build());
  }
}