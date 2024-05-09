using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly FacadeMongoDBRequests requests;

  public DeleteSirenaPlanFactory(FacadeMongoDBRequests requests)
  {
    this.requests = requests;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    Container<SirenRepresentation> container = new Container<SirenRepresentation>();
    CommandStep[] steps = [
      new FindRemoveSirenaStep(contextContainer,container,  requests),
      new DeleteConcretteSirenaStep(contextContainer,container,  requests),
    ];
    return new(steps, contextContainer);
  }
}