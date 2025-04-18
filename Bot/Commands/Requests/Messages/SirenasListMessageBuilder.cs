using Hedgey.Localization;
using Hedgey.Sirena.Entities;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Globalization;
using System.Text;

namespace Hedgey.Sirena.Bot;
public class SirenasListMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider
  , string introductionKey, string commandName)
  : MessageBuilder(chatId, info, localizationProvider)
{
  private IEnumerable<SirenaData>? sirenas = null;
  private readonly string introductionKey = introductionKey;
  private readonly string commandName = commandName;

  public SirenasListMessageBuilder SetSirenas(IEnumerable<SirenaData> sirenas)
  {
    this.sirenas = sirenas;
    return this;
  }
  public override SendMessage Build()
  {
    if (sirenas == null)
      throw new ArgumentNotInitializedException(nameof(sirenas));

    StringBuilder builder = new StringBuilder();

    var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();

    const string template = ". \\[`{0}`] *{1}*\n";
    string listIntroduction = Localize(introductionKey);

    const int maxPerLine = 5;
    var distributor = new ElemTableDistributor(sirenas.Count(), maxPerLine);

    int number = 0;
    builder.AppendLine(listIntroduction).AppendLine();
    foreach (var sirena in sirenas)
    {
      ++number;

      if (distributor.IsRowFilled())
        keyboardBuilder.EndRow().BeginRow();

      keyboardBuilder.AddButton(number, commandName, sirena.ShortHash);

      builder.Append(number).AppendFormat(template, sirena.ShortHash, sirena.Title);
      builder.AppendLine();
    }
    IReplyMarkup replyMarkup = keyboardBuilder.EndRow().ToReplyMarkup();

    var messageText = builder.ToString();
    return CreateDefault(messageText, replyMarkup);
  }
}
public struct ElemTableDistributor
{
  private int extra;
  private int buttonsPerLine;
  private int number;
  private int prevNumber;
  public ElemTableDistributor(int total, int maxPerLine)
  {
    number = 0;
    prevNumber = 0;
    CalcDivisionParams(total,maxPerLine);
  }
  //Evaluate buttons per line
  private void CalcDivisionParams(int total, int maxPerLine)
  {
    int lines = (int)MathF.Ceiling((float)total / maxPerLine);
    if (total > 2 && lines == 1) lines = 2;
    extra = total % lines;
    buttonsPerLine = total / lines + 1;
    if (extra > 0)
      ++buttonsPerLine;
  }

  public bool IsRowFilled()
  {
    ++number;
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
      return true;
    }
    return false;
  }
}