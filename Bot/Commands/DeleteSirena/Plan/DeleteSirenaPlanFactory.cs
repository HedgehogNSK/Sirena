using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFactory<NullableContainer<SirenRepresentation>, FindRemoveSirenaStep> findRemoveSirenaStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, ConfirmationRemoveSirenaStep> removeConfirmationStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, DeleteConcretteSirenaStep> deleteSirenaStepFactory;

  public DeleteSirenaPlanFactory(IFactory<NullableContainer<SirenRepresentation>, FindRemoveSirenaStep> findRemoveSirenaStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, ConfirmationRemoveSirenaStep> removeConfirmationStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, DeleteConcretteSirenaStep> deleteSirenaStepFactory)
  {
    this.findRemoveSirenaStepFactory = findRemoveSirenaStepFactory;
    this.removeConfirmationStepFactory = removeConfirmationStepFactory;
    this.deleteSirenaStepFactory = deleteSirenaStepFactory;
  }

  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<SirenRepresentation> sirenaContainer = new();
    CommandStep[] steps = [
      findRemoveSirenaStepFactory.Create(sirenaContainer),
      removeConfirmationStepFactory.Create(sirenaContainer),
      deleteSirenaStepFactory.Create(sirenaContainer)
    ];
    return new(DeleteSirenaCommand.NAME, steps);
  }
}