using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class AskSirenaIdMessageBuilder : MessageBuilder
{
  public AskSirenaIdMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider)
: base(chatId, info, localizationProvider) { }

  public override SendMessage Build()
  {
    string askSirenaId = Localize("command.subscribe.askSirenaId");

    var markup = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow().AddFindButton(Info).AddMenuButton(Info).EndRow()
        .ToReplyMarkup();

    return CreateDefault(askSirenaId, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, ISendMessageBuilder>
  {

    public ISendMessageBuilder Create(IRequestContext context)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      return new AskSirenaIdMessageBuilder(chatId, info, localizationProvider);
    }
  }
}