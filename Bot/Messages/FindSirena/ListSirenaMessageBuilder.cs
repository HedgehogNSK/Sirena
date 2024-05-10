using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class ListSirenaMessageBuilder : MessageBuilder
{
  private (SirenRepresentation sirena,string ownerName)[] collection;

  public ListSirenaMessageBuilder(long chatId, (SirenRepresentation sirena,string ownerName)[] collection)
  : base(chatId)
  {
    this.collection = collection;
  }

  public override SendMessage Build()
  {
    const string template = ". `{0}` {1}\n*{2}*\n\n";
    StringBuilder builder = new StringBuilder("Found sirenas:\n");
    int number = 1;
    foreach (var tuple in collection)
    {
      var sirena = tuple.sirena;
      var owner = tuple.ownerName;
      builder.Append(number)
      .AppendFormat(template,sirena.Id, owner , sirena.Title);

      ++number;
    }
    const string searchTitle = "🔎 Find another";
    const string menuTitle = "🧾 Back to menu";
    
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddCallbackData(menuTitle, '/' + MenuBotCommand.NAME)
       .AddCallbackData(searchTitle, '/' + FindSirenaCommand.NAME)
       .EndRow();

    var keyboard = new InlineKeyboardMarkup
    {
      InlineKeyboard = keyboardBuilder.Build()
    };

    return CreateDefault(builder.ToString(), keyboard);
  }
}