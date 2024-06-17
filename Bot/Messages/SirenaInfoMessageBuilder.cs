using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class SirenaInfoMessageBuilder : LocalizedMessageBuilder
{
  private readonly long uid;
  private SirenRepresentation sirena;

  public SirenaInfoMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, long uid, SirenRepresentation sirena)
  : base(chatId,info,localizationProvider)
  {
    this.uid = uid;
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    bool userIsOwner = sirena.OwnerId == uid;
    const string template = "Title: *{0}*\nID: `{1}`\nSubscribers: {2}\n";
    const string lastCall = "Last call: {0}\n";

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow();

    if (userIsOwner)
    {
      if (sirena.Listener.Length != 0)
        keyboardBuilder.AddCallSirenaButton(Info, sirena.Id).EndRow().BeginRow();
      if (sirena.Requests.Length != 0)
        keyboardBuilder.AddDisplayRequestsButton(Info,sirena.Id, sirena.Requests.Length);
      if (sirena.Responsible.Length != 0)
        keyboardBuilder.AddDisplayResponsiblesButton(Info,sirena.Id, sirena.Responsible.Length);
      if (sirena.Requests.Length != 0 || sirena.Responsible.Length != 0)
        keyboardBuilder.EndRow().BeginRow();
      keyboardBuilder.AddDeleteButton(Info,sirena.Id);
    }
    else
    {
      bool isSubscribed = sirena.Listener.Contains(uid);
      if (isSubscribed)
      {
        keyboardBuilder.AddUnsubscribeButton(Info,sirena.Id);
      }
      else
      {
        keyboardBuilder.AddSubscribeButton(Info,sirena.Id);
      }
    }
    var markup = keyboardBuilder.AddMenuButton(Info).EndRow().ToReplyMarkup();
    StringBuilder builder = new StringBuilder()
    .AppendFormat(template, sirena.Title, sirena.Id, sirena.Listener.Length);

    if (sirena.LastCall != null)
      builder.AppendFormat(lastCall, sirena.LastCall.Date);

    return CreateDefault(builder.ToString(), markup);
  }
}