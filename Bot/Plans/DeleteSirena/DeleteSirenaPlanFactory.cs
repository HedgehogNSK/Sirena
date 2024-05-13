using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IGetUserRelatedSirenas getUserSirenasOperation;
  private readonly IDeleteSirenaOperation deleteSirenaOperation;

  public DeleteSirenaPlanFactory(IFindSirenaOperation findSirenaOperation
  , IGetUserRelatedSirenas getUserSirenasOperation
  , IDeleteSirenaOperation deleteSirenaOperation)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.getUserSirenasOperation = getUserSirenasOperation;
    this.deleteSirenaOperation = deleteSirenaOperation;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<SirenRepresentation> container = new ();
    CommandStep[] steps = [
      new FindRemoveSirenaStep(contextContainer,container,findSirenaOperation, getUserSirenasOperation),
      new DeleteConcretteSirenaStep(contextContainer,container,deleteSirenaOperation),
    ];
    return new(steps, contextContainer);
  }
}