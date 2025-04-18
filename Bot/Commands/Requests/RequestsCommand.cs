using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class RequestsCommand(PlanScheduler planScheduler
  , IFactory<SirenasListMessageBuilder, NullableContainer<IEnumerable<SirenaData>>, GetUserSirenasStep> loadSirenasStepFactory
  , IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep> idValidationStep
  , IFactory<NullableContainer<ulong>, NullableContainer<SirenaData>, GetUserSirenaStep> loadSirenaStepFactory
  , IFactory<NullableContainer<SirenaData>, DisplaySirenaRequestsStep> displayRequestsStepFactory
  , IFactory<NullableContainer<IEnumerable<SirenaData>>, LoadUserSirenasWithRequestsStep> loadRequestsStepFactory
  , IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory)
  : PlanExecutorBotCommand(NAME, DESCRIPTION, planScheduler)
{
  public const string NAME = "requests";
  public const string DESCRIPTION = "Display a list of requests for permission to launch a sirena.";
  private readonly IFactory<SirenasListMessageBuilder, NullableContainer<IEnumerable<SirenaData>>, GetUserSirenasStep> getUserSirenasStepFactory = loadSirenasStepFactory;
  private readonly IFactory<NullableContainer<ulong>, RequestsValidateSirenaIdStep> idValidationStep = idValidationStep;
  private readonly IFactory<NullableContainer<ulong>, NullableContainer<SirenaData>, GetUserSirenaStep> getSirenaStepFactory = loadSirenaStepFactory;
  private readonly IFactory<NullableContainer<SirenaData>, DisplaySirenaRequestsStep> displayRequestsStepFactory = displayRequestsStepFactory;
  private readonly IFactory<NullableContainer<IEnumerable<SirenaData>>, LoadUserSirenasWithRequestsStep> loadRequestsStepFactory = loadRequestsStepFactory;
  private readonly IFactory<IRequestContext, SirenasListMessageBuilder> userSirenasMessageBuilderFactory = userSirenasMessageBuilderFactory;

  protected override CommandPlan Create(IRequestContext context)
  {


    if (string.IsNullOrEmpty(context.GetArgsString()))
    {
      SirenasListMessageBuilder displayRequestsMessageBuilder = userSirenasMessageBuilderFactory.Create(context);
      NullableContainer<IEnumerable<SirenaData>> sirenasContainer = new();
      var loadSirenasStep = loadRequestsStepFactory.Create(sirenasContainer);
      var getSirenasStep = getUserSirenasStepFactory.Create(displayRequestsMessageBuilder, sirenasContainer);
      return new(NAME, [loadSirenasStep, getSirenasStep]);
    }
    else
    {
      NullableContainer<ulong> idContainer = new();
      NullableContainer<SirenaData> sirenaContaier = new();
      CompositeCommandStep validationChain = new CompositeCommandStep(
        idValidationStep.Create(idContainer),
        getSirenaStepFactory.Create(idContainer, sirenaContaier)
        );
      var displayRequestsStep = displayRequestsStepFactory.Create(sirenaContaier);
      return new(NAME, [validationChain, displayRequestsStep]);
    }
  }

  public static RequestInfo Create(SirenaData sirena, string requestIdString)
  {
    bool isExplicitID = int.TryParse(requestIdString, out int requestID);
    if (isExplicitID)
      requestID = Math.Clamp(requestID, 0, sirena.Requests.Length - 1);

    return new(sirena, isExplicitID, requestID);
  }

  public sealed record RequestInfo(SirenaData Sirena, bool isExplicitID, int RequestID)
  {
    public long RequestorID => Sirena.Requests[RequestID].UID;
    public string Username { get; set; } = string.Empty;
  }
}