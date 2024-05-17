using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class ListSirenaMessageBuilder : MessageBuilder
{
  private (SirenRepresentation sirena, string ownerName)[] collection;

  public ListSirenaMessageBuilder(long chatId, (SirenRepresentation sirena, string ownerName)[] collection)
  : base(chatId)
  {
    this.collection = collection;
  }

  public override SendMessage Build()
  {
    const int buttonsPerLine = 5;
    const string template = ".*{0}* by {1}\n`{2}`\n\n";
    StringBuilder builder = new StringBuilder("Found sirenas:\n");
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    int number = 0;
    foreach (var tuple in collection)
    {
      var sirena = tuple.sirena;
      var owner = tuple.ownerName;

      ++number;

      if (number % buttonsPerLine == 0)
      {
        keyboardBuilder.EndRow().BeginRow();
      }
      keyboardBuilder.AddSirenaInfoButton(tuple.sirena.Id, number.ToString());

      builder.Append(number)
      .AppendFormat(template, sirena.Title, owner, sirena.Id);
    }

    var markup = keyboardBuilder.EndRow().BeginRow()
       .AddFindButton().AddMenuButton().EndRow()
       .ToReplyMarkup();

    return CreateDefault(builder.ToString(), markup);
  }
}