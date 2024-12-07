using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
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
  , ILocalizationProvider localizationProvider, long uid, SirenRepresentation sirena)
  : base(chatId, info, localizationProvider)
  {
    this.uid = uid;
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    bool userIsOwner = sirena.OwnerId == uid;
    const string generalInfoKey = "command.sirena_info.general_info";
    const string generalInfoNotOwnerKey = "command.sirena_info.general_info.not_owner";
    const string lastCallKey = "command.sirena_info.last_call";

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow();

    if (userIsOwner)
    {
      if (sirena.Listener.Length != 0)
        keyboardBuilder.AddCallSirenaButton(Info, sirena.ShortHash).EndRow().BeginRow();
      if (sirena.Requests.Length != 0)
        keyboardBuilder.AddDisplayRequestsButton(Info, sirena.ShortHash, sirena.Requests.Length);
      if (sirena.Responsible.Length != 0)
        keyboardBuilder.AddDisplayResponsiblesButton(Info, sirena.ShortHash, sirena.Responsible.Length);
      if (sirena.Requests.Length != 0 || sirena.Responsible.Length != 0)
        keyboardBuilder.EndRow().BeginRow();
      keyboardBuilder.AddDeleteButton(Info, sirena.ShortHash);
    }
    else
    {
      bool isSubscribed = sirena.Listener.Contains(uid);
      if (isSubscribed)
      {
        bool canCall = sirena.CanBeCalledBy(uid);
        bool canRequest = !sirena.Requests.Any(_request => _request.UID == uid);
        if (canCall)
          keyboardBuilder.AddCallSirenaButton(Info, sirena.ShortHash).EndRow().BeginRow();
        else if (canRequest)
          keyboardBuilder.AddRequestButton(Info, sirena.ShortHash);

        keyboardBuilder.AddUnsubscribeButton(Info, sirena);

        if (canRequest)
          keyboardBuilder.EndRow().BeginRow();
      }
      else
      {
        keyboardBuilder.AddSubscribeButton(Info, sirena.ShortHash);
      }
    }
    var markup = keyboardBuilder.AddMenuButton(Info).EndRow().ToReplyMarkup();
    string generalInfo = Localize(userIsOwner ? generalInfoKey : generalInfoNotOwnerKey);
    StringBuilder builder = new StringBuilder()
        .AppendFormat(generalInfo, sirena.Title, sirena.ShortHash, sirena.Listener.Length, sirena.OwnerNickname);

    if (sirena.LastCall != null)
    {
      string lastCall = Localize(lastCallKey);
      builder.AppendFormat(lastCall, sirena.LastCall.Date);
    }

    return CreateDefault(builder.ToString(), markup);
  }
  public class Factory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, long, SirenRepresentation, SirenaInfoMessageBuilder>
  {
    public SirenaInfoMessageBuilder Create(IRequestContext context, long uid, SirenRepresentation sirena)
    {
      long chatId = context.GetTargetChatId();
      CultureInfo info = context.GetCultureInfo();
      return new SirenaInfoMessageBuilder(chatId, info, localizationProvider, uid, sirena);
    }
  }
}