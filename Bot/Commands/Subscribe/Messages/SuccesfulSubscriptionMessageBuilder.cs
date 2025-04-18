using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SuccesfulSubscriptionMessageBuilder : MessageBuilder
{
  private SirenaData representation;

  public SuccesfulSubscriptionMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider
    , SirenaData representation)
   : base(chatId, info, localizationProvider)
  {
    this.representation = representation;
  }

  public override SendMessage Build()
  {
    IReplyMarkup markup = KeyboardBuilder.CreateInlineKeyboard()
      .BeginRow().AddMenuButton(Info).EndRow()
      .ToReplyMarkup();

    string notificationText = Localize("command.subscribe.success");
    var message = string.Format(notificationText, representation.Title);
    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
   : IFactory<IRequestContext, SirenaData, SuccesfulSubscriptionMessageBuilder>
  {

    public SuccesfulSubscriptionMessageBuilder Create(IRequestContext context, SirenaData sirena)
    {
      CultureInfo info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      return new SuccesfulSubscriptionMessageBuilder(chatId, info, localizationProvider, sirena);
    }
  }
}