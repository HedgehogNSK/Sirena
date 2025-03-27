using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestsCommand(PlanScheduler planScheduler
  , IFactory<SirenasListMessageBuilder, GetUserSirenasStep> getUserSirenasStepFactory
  , IFactory<NullableContainer<ulong>, ISendMessageBuilder, RequestsValidateSirenaIdStep> idValidationStep
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, ISendMessageBuilder, GetUserSirenaStep> getSirenaStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, DisplaySirenaRequestsStep> displayRequestsStepFactory
  , IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory)
  : PlanExecutorBotCommand(NAME, DESCRIPTION, planScheduler)
{
  public const string NAME = "requests";
  public const string DESCRIPTION = "Display a list of requests for permission to launch a sirena.";
  private readonly IFactory<SirenasListMessageBuilder, GetUserSirenasStep> getUserSirenasStepFactory = getUserSirenasStepFactory;
  private readonly IFactory<NullableContainer<ulong>, ISendMessageBuilder, RequestsValidateSirenaIdStep> idValidationStep = idValidationStep;
  private readonly IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, ISendMessageBuilder, GetUserSirenaStep> getSirenaStepFactory = getSirenaStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, DisplaySirenaRequestsStep> displayRequestsStepFactory = displayRequestsStepFactory;
  private readonly IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory = userSirenasMessageBuilderFactory;

  protected override CommandPlan Create(IRequestContext context)
  {
    SirenasListMessageBuilder builder = userSirenasMessageBuilderFactory.Create(context);
    var getSirenasStep = getUserSirenasStepFactory.Create(builder);

    NullableContainer<ulong> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContaier = new();
    CompositeCommandStep validationChain = new CompositeCommandStep(
      idValidationStep.Create(idContainer, builder),
      getSirenaStepFactory.Create(idContainer, sirenaContaier, builder)
      );

    var displayStep = displayRequestsStepFactory.Create(sirenaContaier);
    return new(NAME, [getSirenasStep, validationChain, displayStep]);
  }

  public static RequestInfo Create(SirenRepresentation sirena, string requestIdString)
  {
    bool isExplicitID = int.TryParse(requestIdString, out int requestID);
    if (isExplicitID)
      requestID = Math.Clamp(requestID, 0, sirena.Requests.Length - 1);

    return new(sirena, isExplicitID, requestID);
  }

  public sealed record RequestInfo(SirenRepresentation Sirena, bool isExplicitID, int RequestID)
  {
    public long RequestorID => Sirena.Requests[RequestID].UID;
    public string Username { get; set; } = string.Empty;
  }
}