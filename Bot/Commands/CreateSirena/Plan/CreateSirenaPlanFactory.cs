using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class CreateSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFactory<IRequestContext, CreateMessageBuilder> messageBuilderFactory;
  private readonly IUserInfoOperations userInfo;
  private readonly ICreateSirenaOperationAsync createSirenaAsync;

  public CreateSirenaPlanFactory(
   IFactory<IRequestContext, CreateMessageBuilder> messageBuilderFactory
   , IUserInfoOperations userInfo
   , ICreateSirenaOperationAsync createSirenaAsync)
  {
    this.messageBuilderFactory = messageBuilderFactory;
    this.userInfo = userInfo;
    this.createSirenaAsync = createSirenaAsync;
  }

  public CommandPlan Create(IRequestContext context)
  {
    CreateMessageBuilder messageBuilder = messageBuilderFactory.Create(context);

    NullableContainer<string> titleContainer = new();
    NullableContainer<UserStatistics> statsContainer = new();

    var validation = new CompositeCommandStep([
      new CheckAbilityToCreateSirenaStep(statsContainer,messageBuilder),
      new ValidateTitleCommandStep(messageBuilder,titleContainer),
    ]);

    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      new GetUserCommandStep(userInfo,messageBuilder,statsContainer),
      validation,
      new RequestDBToCommandStep(createSirenaAsync,messageBuilder, titleContainer),
    ];
    return new(CreateSirenaCommand.NAME, steps);
  }
}