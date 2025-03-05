using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class AskSirenaIdForInfoMessageBuilder : MessageBuilder
{
  public AskSirenaIdForInfoMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider)
: base(chatId, info, localizationProvider) { }

  public override SendMessage Build()
  {
    const string messageKey = "command.info.askSirenaId";
    string message = string.Format(Localize(messageKey), Localize(MarkupShortcuts.findTitle));

    var markup = KeyboardBuilder.CreateInlineKeyboard()
        .BeginRow().AddFindButton(Info).AddMenuButton(Info).EndRow()
        .ToReplyMarkup();

    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, ISendMessageBuilder>
  {

    public ISendMessageBuilder Create(IRequestContext context)
    {
      var chatId = context.GetTargetChatId();
      var info = context.GetCultureInfo();
      return new AskSirenaIdForInfoMessageBuilder(chatId, info, localizationProvider);
    }
  }
}