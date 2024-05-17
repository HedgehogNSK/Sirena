using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class SirenaInfoMessageBuilder : MessageBuilder
{
  private readonly long uid;
  private SirenRepresentation sirena;

  public SirenaInfoMessageBuilder(long chatId, long uid, SirenRepresentation sirena)
  : base(chatId)
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
        keyboardBuilder.AddCallSirenaButton(sirena.Id).EndRow().BeginRow();
      if (sirena.Requests.Length != 0)
        keyboardBuilder.AddDisplayRequestsButton(sirena.Id, sirena.Requests.Length);
      if (sirena.Responsible.Length != 0)
        keyboardBuilder.AddDisplayResponsiblesButton(sirena.Id, sirena.Responsible.Length);
      if (sirena.Requests.Length != 0 || sirena.Responsible.Length != 0)
        keyboardBuilder.EndRow().BeginRow();
      keyboardBuilder.AddDeleteButton(sirena.Id);
    }
    else
    {
      bool isSubscribed = sirena.Listener.Contains(uid);
      if (isSubscribed)
      {
        keyboardBuilder.AddUnsubscribeButton(sirena.Id);
      }
      else
      {
        keyboardBuilder.AddSubscribeButton(sirena.Id);
      }
    }
    var markup = keyboardBuilder.AddMenuButton().EndRow().ToReplyMarkup();
    StringBuilder builder = new StringBuilder()
    .AppendFormat(template, sirena.Title, sirena.Id, sirena.Listener.Length);

    if (sirena.LastCall != null)
      builder.AppendFormat(lastCall, sirena.LastCall.Date);

    return CreateDefault(builder.ToString(), markup);
  }
}