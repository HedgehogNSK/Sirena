using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SirenaNotFoundMessageBuilder : LocalizedMessageBuilder
{
  private readonly ulong id;
  public SirenaNotFoundMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, ulong id)
  : base(chatId,info,localizationProvider)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
        .AddFindButton(Info).AddSubscribeButton(Info).AddMenuButton(Info)
        .EndRow().ToReplyMarkup();

    string noSirenaError = Localize("command.subscribe.notExists");
    var message = string.Format(noSirenaError, id);
    return CreateDefault(message, markup);
  }
  
  public class Factory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, ulong, SirenaNotFoundMessageBuilder>
  {
    public SirenaNotFoundMessageBuilder Create(IRequestContext context, ulong id)
    {

      long chatId = context.GetTargetChatId();
      CultureInfo info = context.GetCultureInfo();
      return new SirenaNotFoundMessageBuilder(chatId, info, localizationProvider, id);
    }
  }
}