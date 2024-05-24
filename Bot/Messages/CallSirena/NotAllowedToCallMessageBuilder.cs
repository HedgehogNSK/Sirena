using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class NotAllowedToCallMessageBuilder : MessageBuilder
{
  private SirenRepresentation sirena;
  private long uid;

  public NotAllowedToCallMessageBuilder(long chatId, SirenRepresentation sirena, long uid)
  : base(chatId)
  {
    this.sirena = sirena;
    this.uid = uid;
  }
  public override SendMessage Build()
  {
    const string notification = "You are not allowed to call this Sirena:\n{0}\n";
    const string notOwner = "You can call Sirenas that were created by you or those Sirenas, that the owner gave you the rights to call";
    const string notNow = "Last call was made recently by *{0}* at {1} UTC. Please wait {2} until next try.";

    StringBuilder builder = new StringBuilder();
    builder.AppendFormat(notification, sirena);
    bool cantCalled = !sirena.CanBeCalledBy(uid);
    if (cantCalled)
    {
      builder.Append(notOwner);
    }
    else if (sirena.LastCall != null)
    {
      var timePassed = DateTimeOffset.UtcNow - sirena.LastCall.Date;
      var timeLeft = SirenaStateValidationStep.allowedCallPeriod - timePassed;
      if (timeLeft.Ticks > 0)
      {
        var initiator = sirena.LastCall.Caller == uid ? "*you*" : $"[other person](tg://user?id={uid})";
        var timeLeftString = timeLeft.ToString(@"mm\:ss");
        builder.AppendFormat(notNow, initiator, sirena.LastCall.Date, timeLeftString);
      }
    }

    var keyboardRow = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddDisplayUserSirenasButton();
    if (cantCalled)
      keyboardRow = keyboardRow.AddRequestButton(sirena.Id).EndRow().BeginRow();
    
    var markup = keyboardRow.AddMenuButton().EndRow().ToReplyMarkup();

    return CreateDefault(builder.ToString(), markup);
  }
}