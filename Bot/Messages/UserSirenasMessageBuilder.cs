using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class UserSirenasMessageBuilder : MessageBuilder
{
  private readonly IEnumerable<SirenRepresentation> sirens;

  public UserSirenasMessageBuilder(long chatId, IEnumerable<SirenRepresentation> sirens) : base(chatId)
  {
    this.sirens = sirens;
  }
  public override SendMessage Build()
  {
    const int buttonsPerLine = 5;
    const string template = ". `{0}` *{1}*\n";
    const string subscribers = "_Subscribers[{0}]_\n";
    const string noCreatedSirens = "You haven't created any Sirena yet.";
    const string listIntroduction = "The list of your sirens:\n";

    StringBuilder builder = new StringBuilder();
    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    if (sirens.Any())
    {

      int number = 0;
      builder.Append(listIntroduction);
      foreach (var sirena in sirens)
      {
        ++number;
        if (number % buttonsPerLine == 0)
        {
          keyboardBuilder.EndRow().BeginRow();
        }
        keyboardBuilder.AddSirenaInfoButton(sirena.Id, number.ToString());

        builder.Append(number).AppendFormat(template, sirena.Id, sirena.Title);
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
      var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
          .AddCreateButton().AddMenuButton().EndRow().ToReplyMarkup();
      return CreateDefault(noCreatedSirens, markup);
    }
  }
}