using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class DeleteSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFactory<NullableContainer<SirenaData>, FindRemoveSirenaStep> findRemoveSirenaStepFactory;
  private readonly IFactory<NullableContainer<SirenaData>, ConfirmationRemoveSirenaStep> removeConfirmationStepFactory;
  private readonly IFactory<NullableContainer<SirenaData>, DeleteConcretteSirenaStep> deleteSirenaStepFactory;

  public DeleteSirenaPlanFactory(IFactory<NullableContainer<SirenaData>, FindRemoveSirenaStep> findRemoveSirenaStepFactory
  , IFactory<NullableContainer<SirenaData>, ConfirmationRemoveSirenaStep> removeConfirmationStepFactory
  , IFactory<NullableContainer<SirenaData>, DeleteConcretteSirenaStep> deleteSirenaStepFactory)
  {
    this.findRemoveSirenaStepFactory = findRemoveSirenaStepFactory;
    this.removeConfirmationStepFactory = removeConfirmationStepFactory;
    this.deleteSirenaStepFactory = deleteSirenaStepFactory;
  }

  public CommandPlan Create(IRequestContext context)
  {
    NullableContainer<SirenaData> sirenaContainer = new();
    CommandStep[] steps = [
      findRemoveSirenaStepFactory.Create(sirenaContainer),
      removeConfirmationStepFactory.Create(sirenaContainer),
      deleteSirenaStepFactory.Create(sirenaContainer)
    ];
    return new(DeleteSirenaCommand.NAME, steps);
  }
}