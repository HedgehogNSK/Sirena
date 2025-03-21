using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class IncorrectParameterMessageBuilder : MessageBuilder
{
  public IncorrectParameterMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider)
    : base(chatId, info, localizationProvider) { }

  public override SendMessage Build()
  {
    string errorMessage = Localize("command.delete.ask_sirena_id");
    return CreateDefault(errorMessage);
  }
  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, IncorrectParameterMessageBuilder>
  {

    public IncorrectParameterMessageBuilder Create(IRequestContext context)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new IncorrectParameterMessageBuilder(chatId, info, localizationProvider);
    }
  }
}