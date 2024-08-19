using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using SimpleInjector;

namespace Hedgey.Sirena.Bot;

public class RequestRightsPlanFactory : IFactory<IRequestContext,CommandPlan>
{
  private readonly Container container;

  public RequestRightsPlanFactory(Container container)
  {
    this.container = container;
  }

  public CommandPlan Create(IRequestContext context)
  {

     CompositeCommandStep validationStep = new(
      container.GetInstance<SirenaIdValidationStep>(),
      container.GetInstance<SirenaExistensValidationStep>()
    );

    IObservableStep<IRequestContext,CommandStep.Report>[] steps =[
      validationStep,
      container.GetInstance<AddRequestMessageStep>(),
      container.GetInstance<SendRequestStep>(),
    ];
    return new(RequestRightsCommand.NAME,steps);
  }
}