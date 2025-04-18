using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class SuccesfulDeleteMessageBuilder : MessageBuilder
{
  private SirenaData deletedSirena;

  public SuccesfulDeleteMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, SirenaData deletedSirena)
  : base(chatId, info, localizationProvider)
  {
    this.deletedSirena = deletedSirena;
  }

  public override SendMessage Build()
  {
    string notification = Localize("command.delete.success");
    string message = string.Format(notification, deletedSirena.Title);
    return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }

  public class Factory(ILocalizationProvider localizationProvider) : IFactory<IRequestContext, SirenaData, SuccesfulDeleteMessageBuilder>
  {

    public SuccesfulDeleteMessageBuilder Create(IRequestContext context, SirenaData sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new SuccesfulDeleteMessageBuilder(chatId, info, localizationProvider, sirena);
    }
  }
}