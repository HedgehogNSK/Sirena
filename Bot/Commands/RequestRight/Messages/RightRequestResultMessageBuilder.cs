using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Messages;
using System.Globalization;

namespace Hedgey.Sirena.Bot;

sealed public class RightRequestResultMessageBuilder : LocalizedMessageBuilder
{
  private readonly IRightsRequestOperation.Result result;

  public RightRequestResultMessageBuilder(long chatId, CultureInfo info
  , ILocalizationProvider localizationProvider, IRightsRequestOperation.Result result)
   : base(chatId, info, localizationProvider)
  {
    this.result = result;
  }

  public override SendMessage Build()
  {
    const string failMessage = "command.request_rights.fail";
    const string noChangesMessage = "command.request_rights.already_sent";
    const string successMessage = "command.request_rights.success";

    string message = Localize(!result.isSirenaFound ? failMessage :
      !result.isSuccess ? noChangesMessage : successMessage);

    var markup = MarkupShortcuts.CreateMenuButtonOnlyMarkup(Info);
    return CreateDefault(message, markup);
  }

  public class Factory(ILocalizationProvider localizationProvider)
  : IFactory<IRequestContext, IRightsRequestOperation.Result, RightRequestResultMessageBuilder>
  {
    public RightRequestResultMessageBuilder Create(IRequestContext context, IRightsRequestOperation.Result requestResult)
    {
      var chatId = context.GetChat().Id;
      var info = context.GetCultureInfo();
      return new RightRequestResultMessageBuilder(chatId, info, localizationProvider, requestResult);
    }
  }
}