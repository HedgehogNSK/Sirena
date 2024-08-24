using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

sealed public class RightRequestResultMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, IRightsRequestOperation.Result result
  , SirenRepresentation sirena) 
    : LocalizedMessageBuilder(chatId, info, localizationProvider)
{
  public override SendMessage Build()
  {
    const string failMessage = "command.request_rights.fail";
    const string noChangesMessage = "command.request_rights.already_sent";
    const string successMessage = "command.request_rights.success";

    string message = Localize(!result.isSirenaFound ? failMessage :
      !result.isSuccess ? noChangesMessage : successMessage);
    message = string.Format(message, sirena.Id);
    var markup = MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info);
    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, IRightsRequestOperation.Result, SirenRepresentation, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context
    , IRightsRequestOperation.Result requestResult, SirenRepresentation sirena)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new RightRequestResultMessageBuilder(chatId, info, localizationProvider, requestResult, sirena);
    }
  }
}