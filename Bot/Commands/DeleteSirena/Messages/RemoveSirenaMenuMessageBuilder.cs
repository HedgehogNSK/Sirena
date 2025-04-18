using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RemoveSirenaMenuMessageBuilder : MessageBuilder
{
  private readonly IEnumerable<SirenaData> userSirenas;

  public RemoveSirenaMenuMessageBuilder(long chatId, CultureInfo info
, ILocalizationProvider localizationProvider, IEnumerable<SirenaData> userSirenas)
  : base(chatId, info, localizationProvider)
  {
    this.userSirenas = userSirenas;
  }

  public override SendMessage Build()
  {
    StringBuilder builder = new StringBuilder();
    if (userSirenas.Any())
    {
      const int optionsPerLine = 5;
      string list = Localize("command.delete.header");
      string template = Localize("command.delete.bref_info");
      int number = 0;
      builder.Append(list);

      var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
      foreach (var sirena in userSirenas)
      {
        ++number;
        if (number % optionsPerLine == 0)
          keyboardBuilder = keyboardBuilder.EndRow().BeginRow();

        builder.Append(number).AppendFormat(template, sirena.Title, sirena.ShortHash);
        keyboardBuilder = keyboardBuilder.AddDeleteButton(Info, sirena.ShortHash, number.ToString());
      }
      IReplyMarkup markup = new InlineKeyboardMarkup()
      {
        InlineKeyboard = keyboardBuilder.EndRow().Build()
      };
      return CreateDefault(builder.ToString(), markup);
    }
    else
    {
      string noCreatedSirens = Localize("command.delete.no_sirenas");
      return CreateDefault(noCreatedSirens);
    }
  }
  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, IEnumerable<SirenaData>, RemoveSirenaMenuMessageBuilder>
  {

    public RemoveSirenaMenuMessageBuilder Create(IRequestContext context, IEnumerable<SirenaData> sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new RemoveSirenaMenuMessageBuilder(chatId, info, localizationProvider, sirena);
    }
  }
}