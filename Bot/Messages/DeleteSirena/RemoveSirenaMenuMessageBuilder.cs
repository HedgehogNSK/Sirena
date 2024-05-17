using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;
using System.Reactive.Linq;
using System.Text;

namespace Hedgey.Sirena.Bot;

public class RemoveSirenaMenuMessageBuilder : MessageBuilder
  {
    private readonly IEnumerable<SirenRepresentation> userSirenas;

    public RemoveSirenaMenuMessageBuilder(long chatId, IEnumerable<SirenRepresentation> userSirenas) : base(chatId)
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
        const string template =". *{0}* `{1}`\n";
        int number = 0;
        builder.Append(list);

        var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
        foreach (var sirena in userSirenas)
        {
          ++number;
          if (number % optionsPerLine == 0)
            keyboardBuilder = keyboardBuilder.EndRow().BeginRow();

          builder.Append(number).AppendFormat(template,sirena.Title, sirena.Id);
          keyboardBuilder = keyboardBuilder.AddDeleteButton(sirena.Id,number.ToString());
        }
        IReplyMarkup markup = new InlineKeyboardMarkup()
        {
          InlineKeyboard = keyboardBuilder.EndRow().Build()
        };
        return CreateDefault(builder.ToString(),markup);
      }
      else
      {
        const string noCreatedSirens = "You haven't created any siren yet.";

        return CreateDefault(noCreatedSirens);
      }
    }
    public class StepMessageBuilder(long chatId) : MessageBuilder(chatId)
    {

      public override SendMessage Build()
      {
        throw new NotImplementedException();
      }
    }
  }