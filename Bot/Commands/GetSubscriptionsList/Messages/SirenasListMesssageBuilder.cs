using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class SirenasListMesssageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, IEnumerable<SirenRepresentation> source
  , IMessageStrategy headerKey
  , IMessageStrategy descriptionKey
  , IMessageStrategy emptyListKey
  ) : LocalizedMessageBuilder(chatId, info, localizationProvider)
{
  public override SendMessage Build()
  {
    StringBuilder builder = new StringBuilder();
    if (source.Skip(1).Any())
    {
      const int buttonsPerLine = 5;
      string sirenaInfo = descriptionKey.Get(Info);
      string header = headerKey.Get(Info);

      int number = 0;
      builder.Append(header);
      var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
      foreach (var sirena in source)
      {
        ++number;
        if (number % buttonsPerLine == 0)
        {
          keyboardBuilder.EndRow().BeginRow();
        }
        keyboardBuilder.AddButton(number, DisplaySirenaInfoCommand.NAME, sirena.ShortHash);

        builder.Append(number)
          .AppendFormat(sirenaInfo, sirena.Title, sirena.OwnerNickname, sirena.ShortHash);
      }
      return CreateDefault(builder.ToString(), keyboardBuilder.EndRow().ToReplyMarkup());
    }
    else if (source.Any())
    {
      var sirena = source.First();
      return new SirenaInfoMessageBuilder(chatId, Info, LocalizationProvider, chatId, sirena).Build();
    }
    else
    {
      builder.Append(emptyListKey.Get(Info));
      return CreateDefault(builder.ToString(), MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
    }
  }
}
