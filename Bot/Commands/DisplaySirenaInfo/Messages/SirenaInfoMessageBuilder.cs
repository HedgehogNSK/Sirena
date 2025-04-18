using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SirenaInfoMessageBuilder : MessageBuilder
{
  private readonly long uid;
  private readonly SirenaData sirena;

  public SirenaInfoMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, long uid, SirenaData sirena)
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
    
    bool canCall = sirena.CanBeCalledBy(uid);
    if (userIsOwner)
    {
      if (canCall)
        keyboardBuilder.AddCallSirenaButton(Info, sirena.ShortHash).EndRow().BeginRow();
      if (sirena.Requests.Length != 0)
        keyboardBuilder.AddRequestsButton(Info, sirena.Requests.Length, sirena.ShortHash);
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
   : IFactory<IRequestContext, long, SirenaData, SirenaInfoMessageBuilder>
  {
    public SirenaInfoMessageBuilder Create(IRequestContext context, long uid, SirenaData sirena)
    {
      long chatId = context.GetTargetChatId();
      CultureInfo info = context.GetCultureInfo();
      return new SirenaInfoMessageBuilder(chatId, info, localizationProvider, uid, sirena);
    }
  }
}