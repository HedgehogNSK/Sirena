using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFactory<IRequestContext, CreateMessageBuilder> messageBuilderFactory;
  private readonly IGetUserOperationAsync getUserOperationAsync;
  private readonly ICreateSirenaOperationAsync createSirenaAsync;

  public CreateSirenaPlanFactory(
   IFactory<IRequestContext, CreateMessageBuilder> messageBuilderFactory
   , IGetUserOperationAsync getUserOperationAsync
   , ICreateSirenaOperationAsync createSirenaAsync)
  {
    this.messageBuilderFactory = messageBuilderFactory;
    this.getUserOperationAsync = getUserOperationAsync;
    this.createSirenaAsync = createSirenaAsync;
  }

  public CommandPlan Create(IRequestContext context)
  {
    CreateMessageBuilder messageBuilder = messageBuilderFactory.Create(context);
    Container<CreateMessageBuilder> messageBuilderContainer = new(messageBuilder);

    NullableContainer<string> titleContainer = new();
    NullableContainer<UserRepresentation> userContainer = new();

    var validation = new CompositeCommandStep([
      new CheckAbilityToCreateSirenaStep(userContainer,messageBuilderContainer),
      new ValidateTitleCommandStep(messageBuilderContainer,titleContainer),
    ]);

    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      new GetUserCommandStep(getUserOperationAsync,messageBuilderContainer,userContainer),
      validation,
      new RequestDBToCommandStep(createSirenaAsync,messageBuilderContainer,userContainer, titleContainer),
    ];
    return new(CreateSirenaCommand.NAME, steps);
  }
}