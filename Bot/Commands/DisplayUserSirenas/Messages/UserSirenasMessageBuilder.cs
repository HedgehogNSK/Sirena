using Hedgey.Localization;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class UserSirenasMessageBuilder : LocalizedMessageBuilder
{
  private readonly IEnumerable<SirenRepresentation> sirens;

  public UserSirenasMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, IEnumerable<SirenRepresentation> sirens) : base(chatId, info, localizationProvider)
  {
    this.sirens = sirens;
  }
  public override SendMessage Build()
  {
    const int buttonsPerLine = 5;

    StringBuilder builder = new StringBuilder();
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    if (sirens.Any())
    {
      const string template = ". `{0}` *{1}*\n";
      string subscribers = Localize("command.display_sirenas.subscribers");
      string listIntroduction = Localize("command.display_sirenas.title");

      int number = 0;
      builder.Append(listIntroduction);
      foreach (var sirena in sirens)
      {
        ++number;
        if (number % buttonsPerLine == 0)
        {
          keyboardBuilder.EndRow().BeginRow();
        }
        keyboardBuilder.AddButton(number, DisplaySirenaInfoCommand.NAME, sirena.ShortHash);

        builder.Append(number).AppendFormat(template, sirena.ShortHash, sirena.Title);
        if (sirena.Listener.Length != 0)
          builder.AppendFormat(subscribers, sirena.Listener.Length);
        builder.AppendLine();
      }
      IReplyMarkup replyMarkup = keyboardBuilder.EndRow().ToReplyMarkup();

      var messageText = builder.ToString();
      return CreateDefault(messageText, replyMarkup);
    }
    else
    {
      string noCreatedSirens = Localize("command.display_sirenas.no_created");
      var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
          .AddCreateButton(Info).AddMenuButton(Info).EndRow().ToReplyMarkup();
      return CreateDefault(noCreatedSirens, markup);
    }
  }
}