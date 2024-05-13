using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;

internal class SubscriptionsMesssageBuilder : MessageBuilder
{
  private IEnumerable<(SirenRepresentation sirena, string nickname)> source;

  public SubscriptionsMesssageBuilder(long chatId, IEnumerable<(SirenRepresentation, string)> source)
  : base(chatId)
  {
    this.chatId = chatId;
    this.source = source;
  }

  public override SendMessage Build()
  {
    StringBuilder builder = new StringBuilder();
    if (source.Any())
    {
      const int buttonsPerLine = 5;
      const string template = ". `{0}` {1}\n*{2}*\n\n";
      const string prefix = "Your subscriptions:\n";
      const string notification = "You can press button to 🔕 *UNSUBSCRIBE* from a certain Sirena.";

      int number = 0;
      builder.Append(prefix);
      var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
      foreach (var tuple in source)
      {
        ++number;
        if (number % buttonsPerLine == 0)
        {
          keyboardBuilder.EndRow().BeginRow();
        }
        builder.Append(number)
          .AppendFormat(template, tuple.sirena.Id, tuple.nickname, tuple.sirena.Title);

        keyboardBuilder.AddCallbackData(number.ToString()
        , '/' + UnsubscribeCommand.NAME + ' ' + tuple.sirena.Id.ToString());
      }

      builder.Append(notification);
      return CreateDefault(builder.ToString(), keyboardBuilder.EndRow().ToReplyMarkup());
    }
    else
    {
      const string noSubs = "You don't have any subscription yet.";
      builder.Append(noSubs);
      return CreateDefault(builder.ToString(), MarkupShortcuts.CreateMenuButtonOnlyMarkup());
    }
  }
}
