using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class ListSirenaMessageBuilder : LocalizedMessageBuilder
{
  private (SirenRepresentation sirena, string ownerName)[] collection;

  public ListSirenaMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, (SirenRepresentation sirena, string ownerName)[] collection)
  : base(chatId,info,localizationProvider)
  {
    this.collection = collection;
  }

  public override SendMessage Build()
  {
    const int buttonsPerLine = 5;
    string template =Localize( "command.subscriptions.bref_info");
    StringBuilder builder = new StringBuilder(Localize("command.find.header"));
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
      keyboardBuilder.AddButton(number, DisplaySirenaInfoCommand.NAME, sirena.ShortHash);

      builder.Append(number)
      .AppendFormat(template, sirena.Title, owner, sirena.ShortHash);
    }

    var markup = keyboardBuilder.EndRow().BeginRow()
       .AddFindButton(Info).AddMenuButton(Info).EndRow()
       .ToReplyMarkup();

    return CreateDefault(builder.ToString(), markup);
  }
}