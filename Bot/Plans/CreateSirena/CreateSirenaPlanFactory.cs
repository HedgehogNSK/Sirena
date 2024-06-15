using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IGetUserOperationAsync getUserOperationAsync;
  private readonly ICreateSirenaOperationAsync createSirenAsync;
  private readonly ILocalizationProvider localizationProvider;

  public CreateSirenaPlanFactory(IGetUserOperationAsync getUserOperationAsync
  , ICreateSirenaOperationAsync createSirenAsync
  , ILocalizationProvider localizationProvider)
  {
    this.getUserOperationAsync = getUserOperationAsync;
    this.createSirenAsync = createSirenAsync;
    this.localizationProvider = localizationProvider;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    CreateMessageBuilder messageBuilder = new(ValidateTitleCreateSirenaStep.TITLE_MIN_LENGHT
    , ValidateTitleCreateSirenaStep.TITLE_MAX_LENGHT, localizationProvider, context.GetCultureInfo(), context.GetChat().Id);
    CreateSirenaStep.Buffer buffer = new(messageBuilder);
    CommandStep[] steps = [
      new GetUserCreateSirenaStep(contextContainer,buffer, getUserOperationAsync),
      new CheckAbilityToCreateSirenaStep(contextContainer, buffer),
      new ValidateTitleCreateSirenaStep(contextContainer,buffer),
      new RequestDBToCreateSirenaStep(contextContainer,buffer,createSirenAsync)
    ];
    return new(steps, contextContainer);
  }
}