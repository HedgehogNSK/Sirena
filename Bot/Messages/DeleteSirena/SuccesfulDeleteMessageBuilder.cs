using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;

namespace Hedgey.Sirena.Bot;

public class SuccesfulDeleteMessageBuilder : MessageBuilder
{
  private SirenRepresentation deletedSirena;

  public SuccesfulDeleteMessageBuilder(long chatId, SirenRepresentation deletedSirena)
  :base(chatId)
  {
    this.deletedSirena = deletedSirena;
  }

  public override SendMessage Build()
  {
    const string notification = "Sirena *\"{0}\"* has been deleted";
    string message = string.Format(notification, deletedSirena.Title ) ;
    return CreateDefault(message, MarkupShortcuts.CreateMenuButtonOnlyMarkup());
  }
}