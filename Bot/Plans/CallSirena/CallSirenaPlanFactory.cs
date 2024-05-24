using Hedgey.Sirena.Bot.Operations;
using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;
using Hedgey.Structure.Plan;
using MongoDB.Bson;
using RxTelegram.Bot.Interface.BaseTypes;

namespace Hedgey.Sirena.Bot;

public class CallSirenaPlanFactory : IFactory<IRequestContext, CommandPlan>
{
  private readonly IFindSirenaOperation findSirenaOperation;
  private readonly IMessageSender messageSender;
  private readonly IMessageForwarder messageForwarder;
  private readonly IUpdateSirenaOperation sirenaUpdater;
  private readonly IMessageCopier messageCopier;

  public CallSirenaPlanFactory(IFindSirenaOperation findSirenaOperation
  , IMessageSender messageSender, IMessageForwarder messageForwarder, IMessageCopier messageCopier, IUpdateSirenaOperation sirenaUpdater)
  {
    this.findSirenaOperation = findSirenaOperation;
    this.messageSender = messageSender;
    this.messageForwarder = messageForwarder;
    this.sirenaUpdater = sirenaUpdater;
    this.messageCopier = messageCopier;
  }

  public CommandPlan Create(IRequestContext context)
  {
    Container<IRequestContext> contextContainer = new(context);
    NullableContainer<ObjectId> idContainer = new();
    NullableContainer<SirenRepresentation> sirenaContainer = new();
    NullableContainer<Message> messageContainer = new();
    //Create composition step from different validations
    //Because we have to make all validations for each itteration of input data
    CompositeCommandStep compositeStep = new(
      new SirenaIdValidationStep(contextContainer, idContainer, findSirenaOperation),
      new SirenaExistensValidationStep(contextContainer, idContainer, sirenaContainer, findSirenaOperation),
      new SirenaStateValidationStep(contextContainer, sirenaContainer)
    );
    IObservableStep<CommandStep.Report>[] steps =
    [
      compositeStep,
      new AddExtraInformationStep(contextContainer,sirenaContainer,messageContainer),
      new CallSirenaStep(contextContainer,sirenaContainer,messageContainer,messageSender, messageCopier, sirenaUpdater)
      ];
    return new(steps, contextContainer);
  }
}
