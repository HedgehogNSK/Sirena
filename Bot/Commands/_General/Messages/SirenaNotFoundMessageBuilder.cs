using Hedgey.Blendflake;
using Hedgey.Localization;
using Hedgey.Structure.Factory;
using Hedgey.Utilities;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SirenaNotFoundMessageBuilder : MessageBuilder
{
  private readonly ulong id;
  public SirenaNotFoundMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, ulong id)
  : base(chatId, info, localizationProvider)
  {
    this.id = id;
  }

  public override SendMessage Build()
  {
    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
        .AddFindButton(Info).AddMenuButton(Info)
        .EndRow().ToReplyMarkup();

    string noSirenaError = Localize("command.subscribe.not_exists");
    var shortId = HashUtilities.Shortify(NotBase64URL.From(id));
    var message = string.Format(noSirenaError, shortId);
    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, ulong, ISendMessageBuilder>
  {
    public ISendMessageBuilder Create(IRequestContext context, ulong id)
    {

      long chatId = context.GetTargetChatId();
      CultureInfo info = context.GetCultureInfo();
      return new SirenaNotFoundMessageBuilder(chatId, info, localizationProvider, id);
    }
  }
}