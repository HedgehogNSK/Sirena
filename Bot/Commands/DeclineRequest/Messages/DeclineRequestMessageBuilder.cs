using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Messages;
using System.Globalization;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public sealed class DeclineRequestMessageBuilder(
  NotAllowedTextGetter.Factory notAllowed
  , NoRequestsTextGetter.Factory noRequests
  , NoRequestorTextGetter.Factory noRequestor
  , SuccesfulDeclineTextGetter.Factory success) : GeneralMessageBuilder
{

  public void NotAllowed(IRequestContext context, SirenaData sirena)
  => SetupTextGetter(notAllowed, context, sirena);

  public void NoRequestor(IRequestContext context, SirenaData sirena, string userName)
    => SetupTextGetter(noRequestor, context, sirena, userName);

  public void NoRequests(IRequestContext context, SirenaData sirena)
    => SetupTextGetter(noRequests, context, sirena);

  public void Success(IRequestContext context, SirenaData sirena, string userName)
    => SetupTextGetter(success, context, sirena, userName);

  public void SetupTextGetter(
    IFactory<CultureInfo, SirenaData, SirenaTemplateTextGetter> textGetterFactory
  , IRequestContext context, SirenaData sirena)
  {
    var info = context.GetCultureInfo();
    var getter = textGetterFactory.Create(info, sirena);
    _ = Set(context).Set(getter);
  }
  public void SetupTextGetter(
    IFactory<CultureInfo, SirenaData, object, SirenaWithExtrasTemplateTextGetter> textGetterFactory
    , IRequestContext context, SirenaData sirena, object extra)
  {
    var info = context.GetCultureInfo();
    var getter = textGetterFactory.Create(info, sirena, extra);
    _ = Set(context).Set(getter);
  }
  public sealed class Factory(NotAllowedTextGetter.Factory notAllowed
  , NoRequestsTextGetter.Factory noRequests
  , NoRequestorTextGetter.Factory noRequestor
  , SuccesfulDeclineTextGetter.Factory success)
  : IFactory<DeclineRequestMessageBuilder>
  {
    public DeclineRequestMessageBuilder Create()
      => new DeclineRequestMessageBuilder(notAllowed, noRequests, noRequestor
        , success);
  }
}