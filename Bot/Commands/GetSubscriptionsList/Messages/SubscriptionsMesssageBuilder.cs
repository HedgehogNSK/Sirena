using Hedgey.Localization;
using Hedgey.Sirena.Database;
using MongoDB.Driver;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;
internal class SubscriptionsMesssageBuilder : LocalizedMessageBuilder
{
  private IEnumerable<(SirenRepresentation sirena, string nickname)> source;

  public SubscriptionsMesssageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider, IEnumerable<(SirenRepresentation, string)> source)
  : base(chatId,info,localizationProvider)
  {
    this.chatId = chatId;
    this.source = source;
  }

  public override SendMessage Build()
  {
    StringBuilder builder = new StringBuilder();
    if (source.Skip(1).Any())
    {
      const int buttonsPerLine = 5;
      string sirenaInfo = Localize("command.subscriptions.bref_info");
      string header = Localize("command.subscriptions.header");

      int number = 0;
      builder.Append(header);
      var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
      foreach (var tuple in source)
      {
        ++number;
        if (number % buttonsPerLine == 0)
        {
          keyboardBuilder.EndRow().BeginRow();
        }
        keyboardBuilder.AddSirenaInfoButton(Info,tuple.sirena.Id,number.ToString());
        
        builder.Append(number)
          .AppendFormat(sirenaInfo, tuple.sirena.Title, tuple.nickname, tuple.sirena.Id);

      }
      return CreateDefault(builder.ToString(), keyboardBuilder.EndRow().ToReplyMarkup());
    }
    else if(source.Any()){
      var subscription =source.First();
      return new SirenaInfoMessageBuilder(chatId,Info, LocalizationProvider, chatId, subscription.sirena).Build();
    }
    else
    {
      string noSubs = Localize("command.subscriptions.noSubscriptions");
      builder.Append(noSubs);
      return CreateDefault(builder.ToString(),  MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info));
    }
  }
}
