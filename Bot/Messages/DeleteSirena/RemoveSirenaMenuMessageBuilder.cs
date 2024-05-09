using System.Reactive.Linq;
using System.Text;
using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Base.Interfaces;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using RxTelegram.Bot.Utils.Keyboard;

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
        int number = 0;
        const string list = "Select Sirena you want to *DELETE*:\n";
        builder.Append(list);

        var keyboardBuilder = KeyboardBuilder.CreateInlineKeyboard().BeginRow();
        foreach (var siren in userSirenas)
        {
          ++number;
          if (number % optionsPerLine == 0)
            keyboardBuilder = keyboardBuilder.EndRow().BeginRow();

          builder.Append(number).Append(' ')          
          .Append('`').Append(siren.Id).Append("` ")
          .Append(siren.Title)
          .Append("\n");
          string command = '/' + DeleteSirenaCommand.NAME+ ' ' + siren.Id;
          keyboardBuilder = keyboardBuilder.AddCallbackData(number.ToString(),command );
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