using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;

namespace Hedgey.Sirena.Bot;

public class ReuquestsPlanFactory(
   IFactory<SirenasListMessageBuilder, GetUserSirenasStep> getUserSirenasStepFactory
  , IFactory<NullableContainer<ulong>, ISendMessageBuilder, ValidateSirenaIdStep2> idValidationStep
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, ISendMessageBuilder, GetUserSirenaStep> getSirenaStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, DisplaySirenaRequestsStep> displayRequestsStepFactory
  , IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory
  ) : IFactory<IRequestContext, CommandPlan>
{
  public CommandPlan Create(IRequestContext context)
  {
    SirenasListMessageBuilder builder = userSirenasMessageBuilderFactory.Create(context);
    var getSirenasStep = getUserSirenasStepFactory.Create(builder);

    NullableContainer<ulong> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContaienr = new();
    IObservableStep<IRequestContext, CommandStep.Report>[] steps = [
      idValidationStep.Create(idContainer,builder),
      getSirenaStepFactory.Create(idContainer,sirenaContaienr,builder),
     ];

    CompositeCommandStep validationChain = new CompositeCommandStep(steps);
    var displayStep = displayRequestsStepFactory.Create(sirenaContaienr);
    return new(RequestsCommand.NAME, [getSirenasStep, validationChain, displayStep]);
  }
}