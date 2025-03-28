using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class DeclineRequestCommand : PlanExecutorBotCommand
{
  public const string NAME = "decline_request";
  const string DESCRIPTION = "Declines a user's request for Siren activation rights.";
  private readonly IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep> validateIdStepFactory;
  private readonly IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep> sirenExistensValidationStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, DeclineRequestStep> declineRequestStepFactory;

  public DeclineRequestCommand(IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep> validateIdStepFactory
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, SirenaExistensValidationStep> sirenExistensValidationStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, DeclineRequestStep> declineRequestStepFactory
  , PlanScheduler planScheduler)
   : base(NAME, DESCRIPTION, planScheduler)
  {
    this.validateIdStepFactory = validateIdStepFactory;
    this.sirenExistensValidationStepFactory = sirenExistensValidationStepFactory;
    this.declineRequestStepFactory = declineRequestStepFactory;
  }
  protected override CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ulong> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContainer = new();
    CompositeCommandStep validationBulkStep = new(
      validateIdStepFactory.Create(idContainer),
      sirenExistensValidationStepFactory.Create(idContainer, sirenaContainer)
    );
    var declineRequestStep = declineRequestStepFactory.Create(sirenaContainer);
    return new CommandPlan(Command, [validationBulkStep, declineRequestStep]);
  }
}