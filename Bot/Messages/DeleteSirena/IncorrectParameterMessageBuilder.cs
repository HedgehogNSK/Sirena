using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class IncorrectParameterMessageBuilder : LocalizedMessageBuilder
  {
    const string errorMessage = "To delete a Sirena, please insert *Sirena's ID or serial number* that is owned by you.";
    public IncorrectParameterMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider)
    : base(chatId,info,localizationProvider) { }

    public override SendMessage Build()
    {
      return CreateDefault(errorMessage);
    }
  }