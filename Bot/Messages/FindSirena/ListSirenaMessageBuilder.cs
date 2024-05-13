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
    const string notification = "To subscribe:\n1. Click on the ID to copy it.\n2. Press *Subscribe* button.\n3. Paste the Sirena ID and press send.\n _Alternatively you can subscribe to a Sirena using the command:_\n`/subscribe sirena_id`";
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

    builder.Append(notification);

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
       .AddSubscribeButton()
       .AddFindButton()
       .AddMenuButton()
       .EndRow();

    var keyboard = new InlineKeyboardMarkup
    {
      InlineKeyboard = keyboardBuilder.Build()
    };

    return CreateDefault(builder.ToString(), keyboard);
  }
}