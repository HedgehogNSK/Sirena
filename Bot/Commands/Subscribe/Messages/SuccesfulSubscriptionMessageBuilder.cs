using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SuccesfulSubscriptionMessageBuilder : LocalizedMessageBuilder
{
  private SirenRepresentation representation;

  public SuccesfulSubscriptionMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider
    , SirenRepresentation representation)
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
   : IFactory<IRequestContext, SirenRepresentation, SuccesfulSubscriptionMessageBuilder>
  {

    public SuccesfulSubscriptionMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
    {
      CultureInfo info = context.GetCultureInfo();
      long chatId = context.GetTargetChatId();
      return new SuccesfulSubscriptionMessageBuilder(chatId, info, localizationProvider, sirena);
    }
  }
}