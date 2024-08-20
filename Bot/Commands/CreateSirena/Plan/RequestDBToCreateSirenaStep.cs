using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

public class RequestDBToCommandStep : CommandStep
{
  private readonly ICreateSirenaOperationAsync createSirenAsync;
  private readonly CreateMessageBuilder messageBuilder;
  private readonly NullableContainer<UserRepresentation> userContainer;
  private readonly NullableContainer<string> titleContainer;

  public RequestDBToCommandStep(ICreateSirenaOperationAsync createSirenAsync
  , CreateMessageBuilder messageBuilder
  , NullableContainer<UserRepresentation> userContainer
  , NullableContainer<string> titleContainer)
: base()
  {
    this.createSirenAsync = createSirenAsync;
    this.messageBuilder = messageBuilder;
    this.userContainer = userContainer;
    this.titleContainer = titleContainer;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    return Observable.FromAsync(()
      =>
    {
      var user = userContainer.Get();
      var sirenaTitle = titleContainer.Get();
      return createSirenAsync.CreateAsync(user.UID, sirenaTitle);
    })
    .Select(CreateReport);
  }

  private Report CreateReport(SirenRepresentation sirena)
  {
    messageBuilder.SetSirena(sirena);
    return new(sirena != null ? Result.Success : Result.Canceled, messageBuilder);
  }
}