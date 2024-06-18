using Hedgey.Localization;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

public class IncorrectParameterMessageBuilder : LocalizedMessageBuilder
  {
  public IncorrectParameterMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider  localizationProvider)
    : base(chatId,info,localizationProvider) { }

    public override SendMessage Build()
    {
    string errorMessage = Localize("command.delete.ask_sirena_id");
    return CreateDefault(errorMessage);
    }
  }