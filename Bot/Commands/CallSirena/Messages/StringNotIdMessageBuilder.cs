using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Text;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class StringNotIdMessageBuilder : MessageBuilder
{
  private readonly IEnumerable<SirenaData> sirens;

  public StringNotIdMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider
  , IEnumerable<SirenaData> sirens)
  : base(chatId, info, localizationProvider)
  {
    this.sirens = sirens;
  }

  public override SendMessage Build()
  {
    StringBuilder builder = new StringBuilder();

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
    if (sirens.Any())
    {
      return DisplayAvailableSirenas(builder, keyboardBuilder);
    }
    else
    {
      string noAvailable = Localize("command.call.no_available");
      var markup = KeyboardBuilder.CreateInlineKeyboard().BeginRow()
          .AddCreateButton(Info).AddMenuButton(Info).EndRow().ToReplyMarkup();
      return CreateDefault(noAvailable, markup);
    }
  }

  private SendMessage DisplayAvailableSirenas(StringBuilder builder, RxTelegram.Bot.Utils.Keyboard.Interfaces.IInlineKeyboardRow keyboardBuilder)
  {
    const string template = ". `{0}` *{1}*\n";
    string subscribers = Localize("command.display_sirenas.subscribers");
    string listIntroduction = Localize("command.call.available.description");

    //Evaluate buttons per line
    const float maxPerLine = 5f;
    int total = sirens.Count();
    int lines = (int)MathF.Ceiling(total / maxPerLine);
    if (total > 2 && lines == 1) lines = 2;
    int extra = total % lines;
    int buttonsPerLine = total / lines + 1;
    if (extra > 0)
      ++buttonsPerLine;

    int number = 0;
    int prevNumber = 0;
    builder.AppendLine(listIntroduction).AppendLine();
    foreach (var sirena in sirens)
    {
      ++number;
      //If the row is full
      if ((number - prevNumber) % buttonsPerLine == 0)
      {
        //If long rows not null
        if (extra > 0)
        {
          --extra;
          //When extras ends up decrease buttonsPerLine
          if (extra == 0)
          {
            --buttonsPerLine;
            prevNumber = number;
          }
        }
        keyboardBuilder.EndRow().BeginRow();
      }
      keyboardBuilder.AddCallbackButton(number, CallSirenaCommand.NAME, sirena.ShortHash);

      builder.Append(number).AppendFormat(template, sirena.ShortHash, sirena.Title);
      if (sirena.Listener.Length != 0)
        builder.AppendFormat(subscribers, sirena.Listener.Length);
      builder.AppendLine();
    }
    IReplyMarkup replyMarkup = keyboardBuilder.EndRow().ToReplyMarkup();

    var messageText = builder.ToString();
    return CreateDefault(messageText, replyMarkup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, IEnumerable<SirenaData>, StringNotIdMessageBuilder>
  {
    public StringNotIdMessageBuilder Create(IRequestContext context, IEnumerable<SirenaData> param)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new StringNotIdMessageBuilder(chatId, info, localizationProvider, param);
    }
  }
}