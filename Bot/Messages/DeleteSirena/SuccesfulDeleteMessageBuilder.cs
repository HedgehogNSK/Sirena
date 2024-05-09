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
    string message = $"Sirena *\"" + deletedSirena.Title + "\"* has been deleted" ;
    return CreateDefault(message);
  }
}