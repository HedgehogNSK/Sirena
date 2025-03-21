using Hedgey.Sirena.Database;
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

  public void NotAllowed(IRequestContext context, SirenRepresentation sirena)
  => SetupTextGetter(notAllowed, context, sirena);

  public void NoRequestor(IRequestContext context, SirenRepresentation sirena, string userName)
    => SetupTextGetter(noRequestor, context, sirena, userName);

  public void NoRequests(IRequestContext context, SirenRepresentation sirena)
    => SetupTextGetter(noRequests, context, sirena);

  public void Success(IRequestContext context, SirenRepresentation sirena, string userName)
    => SetupTextGetter(success, context, sirena, userName);

  public void SetupTextGetter(
    IFactory<CultureInfo, SirenRepresentation, SirenaTemplateTextGetter> textGetterFactory
  , IRequestContext context, SirenRepresentation sirena)
  {
    var info = context.GetCultureInfo();
    var getter = textGetterFactory.Create(info, sirena);
    _ = Set(context).Set(getter);
  }
  public void SetupTextGetter(
    IFactory<CultureInfo, SirenRepresentation, object, SirenaWithExtrasTemplateTextGetter> textGetterFactory
    , IRequestContext context, SirenRepresentation sirena, object extra)
  {
    var info = context.GetCultureInfo();
    var getter = textGetterFactory.Create(info, sirena, extra);
    _ = Set(context).Set(getter);
  }
}