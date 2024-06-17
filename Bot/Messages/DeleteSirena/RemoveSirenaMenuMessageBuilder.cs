using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class RemoveSirenaMenuMessageBuilder : LocalizedMessageBuilder
{
  private readonly IEnumerable<SirenRepresentation> userSirenas;

  public RemoveSirenaMenuMessageBuilder(long chatId, CultureInfo info
, ILocalizationProvider localizationProvider, IEnumerable<SirenRepresentation> userSirenas) : base(chatId, info, localizationProvider)
  {
    this.userSirenas = userSirenas;
  }

  public override SendMessage Build()
  {
    StringBuilder builder = new StringBuilder();
    if (userSirenas.Any())
    {
      const int optionsPerLine = 5;
      const string list = "Select Sirena you want to *DELETE*:\n";
      const string template = ". *{0}* `{1}`\n";
      int number = 0;
      builder.Append(list);

      var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
      foreach (var sirena in userSirenas)
      {
        ++number;
        if (number % optionsPerLine == 0)
          keyboardBuilder = keyboardBuilder.EndRow().BeginRow();

        builder.Append(number).AppendFormat(template, sirena.Title, sirena.Id);
        keyboardBuilder = keyboardBuilder.AddDeleteButton(Info, sirena.Id, number.ToString());
      }
      IReplyMarkup markup = new InlineKeyboardMarkup()
      {
        InlineKeyboard = keyboardBuilder.EndRow().Build()
      };
      return CreateDefault(builder.ToString(), markup);
    }
    else
    {
      const string noCreatedSirens = "You haven't created any siren yet.";

      return CreateDefault(noCreatedSirens);
    }
  }
}