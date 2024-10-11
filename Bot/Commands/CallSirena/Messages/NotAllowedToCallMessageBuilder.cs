using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class NotAllowedToCallMessageBuilder : LocalizedMessageBuilder
{
  private SirenRepresentation sirena;
  private long uid;

  public NotAllowedToCallMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, SirenRepresentation sirena, long uid)
  : base(chatId, info, localizationProvider)
  {
    this.sirena = sirena;
    this.uid = uid;
  }
  public override SendMessage Build()
  {
    string notification = Localize("command.call.not_allowed");
    string notOwner = Localize("command.call.explanation_rights");
    string notNow = Localize("command.call.last_call");

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
        var initiator = sirena.LastCall.Caller == uid ? "command.call.user"
         : "command.call.other";
        initiator = Localize(initiator);
        initiator = string.Format(initiator, uid);

        var timeLeftString = timeLeft.ToString(@"mm\:ss");

        builder.AppendFormat(notNow, initiator, sirena.LastCall.Date, timeLeftString);
      }
    }

    var keyboardRow = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
      .AddDisplayUserSirenasButton(Info);
    if (cantCalled)
      keyboardRow = keyboardRow.AddRequestButton(Info, sirena.Sid).EndRow().BeginRow();

    var markup = keyboardRow.AddMenuButton(Info).EndRow().ToReplyMarkup();

    return CreateDefault(builder.ToString(), markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, NotAllowedToCallMessageBuilder>
  {

    public NotAllowedToCallMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      long uid = context.GetUser().Id;
      return new NotAllowedToCallMessageBuilder(chatId, info, localizationProvider, sirena, uid);
    }
  }
}