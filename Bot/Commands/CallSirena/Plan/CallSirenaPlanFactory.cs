using Hedgey.Localization;
using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class CallSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly NullableContainer<ObjectId> objectIdContainer;
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly NullableContainer<Message> messageContainer;
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IUpdateSirenaOperation sirenaUpdater;
  private readonly IMessageSender messageSender;
  private readonly IMessageCopier messageCopier;
  private readonly ILocalizationProvider localizationProvider;

  public CallSirenaPlanFactory(NullableContainer<ObjectId> objectIdContainer
  , NullableContainer<SirenRepresentation> sirenaContainer
  , NullableContainer<Message> messageContainer
  , IFindSirenaOperation findSirenaOperation
  , IUpdateSirenaOperation sirenaUpdater
  , IMessageSender messageSender
  , IMessageCopier messageCopier
  , ILocalizationProvider localizationProvider)
  {
    this.objectIdContainer = objectIdContainer;
    this.sirenaContainer = sirenaContainer;
    this.messageContainer = messageContainer;
    this.findSirenaOperation = findSirenaOperation;
    this.sirenaUpdater = sirenaUpdater;
    this.messageSender = messageSender;
    this.messageCopier = messageCopier;
    this.localizationProvider = localizationProvider;
  }


  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new Container<IRequestContext>(context);

    //Create composition step from different validations
    //Because we have to make all validations for each itteration of input data
    CompositeCommandStep compositeStep = new(
      new SirenaIdValidationStep(contextContainer,localizationProvider, objectIdContainer),
      new SirenaExistensValidationStep(contextContainer, objectIdContainer, sirenaContainer, findSirenaOperation, localizationProvider),
      new SirenaStateValidationStep(contextContainer, sirenaContainer, localizationProvider)
    );
    IObservableStep<CommandStep.Report>[] steps =
    [
      compositeStep,
      new AddExtraInformationStep(contextContainer,sirenaContainer,messageContainer, localizationProvider),
      new CallSirenaStep(contextContainer,sirenaContainer,messageContainer,messageSender, messageCopier, sirenaUpdater, localizationProvider)
      ];
    return new(steps, contextContainer);
  }
}
