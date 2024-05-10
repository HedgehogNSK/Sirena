using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IFindUserSirenasOperation findUsersSirenaOperation;
  private readonly IDeleteSirenaOperation deleteSirenaOperation;

  public DeleteSirenaPlanFactory(IFindSirenaOperation findSirenaOperation
  , IFindUserSirenasOperation findUsersSirenaOperation
  , IDeleteSirenaOperation deleteSirenaOperation)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.findUsersSirenaOperation = findUsersSirenaOperation;
    this.deleteSirenaOperation = deleteSirenaOperation;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    Container<SirenRepresentation> container = new Container<SirenRepresentation>();
    CommandStep[] steps = [
      new FindRemoveSirenaStep(contextContainer,container,findSirenaOperation, findUsersSirenaOperation),
      new DeleteConcretteSirenaStep(contextContainer,container,deleteSirenaOperation),
    ];
    return new(steps, contextContainer);
  }
}