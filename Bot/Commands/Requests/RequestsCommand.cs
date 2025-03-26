using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestsCommand(PlanScheduler planScheduler
  , IFactory<SirenasListMessageBuilder, GetUserSirenasStep> getUserSirenasStepFactory
  , IFactory<NullableContainer<ulong>, ISendMessageBuilder, RequestsValidateSirenaIdStep> idValidationStep
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, ISendMessageBuilder, GetUserSirenaStep> getSirenaStepFactory
  , IFactory<NullableContainer<SirenRepresentation>, NullableContainer<RequestsCommand.RequestInfo>, CreateRequestInfoStep> createRequestInfoStepFactory
  , IFactory<NullableContainer<RequestsCommand.RequestInfo>, DisplaySirenaRequestsStep> displayRequestsStepFactory
  , IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory) 
  : PlanExecutorBotCommand(NAME, DESCRIPTION, planScheduler)
{
  public const string NAME = "requests";
  public const string DESCRIPTION = "Display a list of requests for permission to launch a sirena.";
  private readonly IFactory<SirenasListMessageBuilder, GetUserSirenasStep> getUserSirenasStepFactory = getUserSirenasStepFactory;
  private readonly IFactory<NullableContainer<ulong>, ISendMessageBuilder, RequestsValidateSirenaIdStep> idValidationStep = idValidationStep;
  private readonly IFactory<NullableContainer<ulong>, NullableContainer<SirenRepresentation>, ISendMessageBuilder, GetUserSirenaStep> getSirenaStepFactory = getSirenaStepFactory;
  private readonly IFactory<NullableContainer<SirenRepresentation>, NullableContainer<RequestInfo>, CreateRequestInfoStep> createRequestInfoStepFactory = createRequestInfoStepFactory;
  private readonly IFactory<NullableContainer<RequestInfo>, DisplaySirenaRequestsStep> displayRequestsStepFactory = displayRequestsStepFactory;
  private readonly IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory = userSirenasMessageBuilderFactory;

  protected override CommandPlan Create(IRequestContext context)
  {
    SirenasListMessageBuilder builder = userSirenasMessageBuilderFactory.Create(context);
    var getSirenasStep = getUserSirenasStepFactory.Create(builder);

    NullableContainer<ulong> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContaier = new();
    NullableContainer<RequestInfo> requestInfoContainer = new();
    CompositeCommandStep validationChain = new CompositeCommandStep(
      idValidationStep.Create(idContainer, builder),
      getSirenaStepFactory.Create(idContainer, sirenaContaier, builder),
      createRequestInfoStepFactory.Create(sirenaContaier, requestInfoContainer)
      );

    var displayStep = displayRequestsStepFactory.Create(requestInfoContainer);
    return new(NAME, [getSirenasStep, validationChain, displayStep]);
  }

  public sealed record RequestInfo(SirenRepresentation Sirena, bool isExplicitID, int RequestID)
  {
    public long RequestorID => Sirena.Requests[RequestID].UID;
  }

public static RequestInfo Create(SirenRepresentation sirena, string requestNumberString)
  {
    bool isExplicitID = int.TryParse(requestNumberString, out int requestID);
    if (isExplicitID)
      requestID = Math.Clamp(requestID, 0, sirena.Requests.Length - 1);

    return new(sirena, isExplicitID, requestID);
  }
}