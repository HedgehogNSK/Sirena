using Hedgey.Localization;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class NotAllowedToCallSirenaMessageBuilder : LocalizedMessageBuilder
{
  private SirenRepresentation sirena;

  public NotAllowedToCallSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, SirenRepresentation sirena)
  : base(chatId, info, localizationProvider)
  {
    this.sirena = sirena;
  }

  public override SendMessage Build()
  {
    string notification = Localize("command.call.not_allowed"); ;
    string message = string.Format(notification, sirena.Title, sirena.Hash);

    var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
    .AddDisplayUserSirenasButton(Info)
    .AddDisplaySubscriptionsButton(Info).EndRow()
    .BeginRow().AddMenuButton(Info).EndRow().ToReplyMarkup();

    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, SirenRepresentation, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context, SirenRepresentation sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new NotAllowedToCallSirenaMessageBuilder(chatId, info, localizationProvider, sirena);
    }
  }
}