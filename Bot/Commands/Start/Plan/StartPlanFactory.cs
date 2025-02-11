using Hedgey.Blendflake;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class StartPlanFactory(IFactory<NullableContainer<UpdateState>, WelcomeMessageStep> welcomeMessageStepFactory
, IFactory<NullableContainer<ulong>, ValidateSirenaIdStep> idValidationStepFactory
, IFactory<NullableContainer<ulong>, RequestSubscribeStep> requestSubscribeStepFactory
, IFactory<NullableContainer<UpdateState>, CreateUserStep> createUserStepFactory)
 : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFactory<NullableContainer<UpdateState>, WelcomeMessageStep> welcomeMessageStepFactory = welcomeMessageStepFactory;
  private readonly IFactory<NullableContainer<ulong>, ValidateSirenaIdStep> idValidationStepFactory = idValidationStepFactory;
  private readonly IFactory<NullableContainer<ulong>, RequestSubscribeStep> requestSubscribeStepFactory = requestSubscribeStepFactory;
  private readonly IFactory<NullableContainer<UpdateState>, CreateUserStep> createUserStepFactory = createUserStepFactory;

  public CommandPlan Create(IRequestContext context)
  {
    var arg = context.GetArgsString();
    NullableContainer<UpdateState> stateContainer = new();
    var createUserStep = createUserStepFactory.Create(stateContainer);
    var welcomeStep = welcomeMessageStepFactory.Create(stateContainer);
    if (!HashUtilities.TryParse(arg, out ulong id))
      return new(StartCommand.NAME, [createUserStep, welcomeStep]);

    Console.WriteLine("Start with subscription id:" + id);
    NullableContainer<ulong> idContainer = new();
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      idValidationStepFactory.Create(idContainer),
      requestSubscribeStepFactory.Create(idContainer),
    ];
    var compositeStep = new CompositeCommandStep(steps);
    return new(StartCommand.NAME, [createUserStep, welcomeStep, compositeStep]);
  }
}