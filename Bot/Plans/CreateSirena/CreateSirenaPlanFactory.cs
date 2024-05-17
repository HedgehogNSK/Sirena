using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IGetUserOperationAsync getUserOperationAsync;
  private readonly ICreateSirenaOperationAsync createSirenAsync;

  public CreateSirenaPlanFactory(IGetUserOperationAsync getUserOperationAsync, ICreateSirenaOperationAsync createSirenAsync)
  {
    this.getUserOperationAsync = getUserOperationAsync;
    this.createSirenAsync = createSirenAsync;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    CreateMessageBuilder messageBuilder = new(context.GetChat().Id);
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