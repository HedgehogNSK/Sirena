using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;
using MongoDB.Bson;

namespace Hedgey.Sirena.Bot;

public class ReactToSirenaCommand : PlanExecutorBotCommand
{
  public const string NAME = "react";
  const string DESCRIPTION = "React to a certain call of the sirena.";
  private readonly IFactory<NullableContainer<ObjectId>, SetReactionStep> setReactionStepFactory;

  public ReactToSirenaCommand(PlanScheduler planScheduler
  , IFactory<NullableContainer<ObjectId>, SetReactionStep> setReactionStepFactory)
   : base(NAME, DESCRIPTION, planScheduler)
  {
    this.setReactionStepFactory = setReactionStepFactory;
  }

  protected override CommandPlan Create(IRequestContext context)
  {
    NullableContainer<ObjectId> idContainer = new();
    ValidateRequestIsFromBot validateSourceStep = new();
    ValidateCallIdStep validateStep = new(idContainer);
    SetReactionStep setReactionStep = setReactionStepFactory.Create(idContainer);
    return new(NAME, [validateSourceStep, validateStep, setReactionStep]);
  }
}