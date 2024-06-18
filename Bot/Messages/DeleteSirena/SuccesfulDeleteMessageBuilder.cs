using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class SuccesfulDeleteMessageBuilder : LocalizedMessageBuilder
{
  private SirenRepresentation deletedSirena;

  public SuccesfulDeleteMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, SirenRepresentation deletedSirena)
  : base(chatId, info, localizationProvider)
  {
    this.deletedSirena = deletedSirena;
  }

  public override SendMessage Build()
  {
    string notification = Localize("command.delete.success");
    string message = string.Format(notification, deletedSirena.Title ) ;
    return CreateDefault(message,  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
  }
}