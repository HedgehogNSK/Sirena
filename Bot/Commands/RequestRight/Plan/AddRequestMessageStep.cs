using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class AddRequestMessageStep : NotificationStep
{
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly IFactory<IRequestContext, SirenRepresentation, IMessageBuilder> messageBuilderFactory;

  public AddRequestMessageStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFactory<IRequestContext, SirenRepresentation
  , IMessageBuilder> messageBuilderFactory)
  {
    this.sirenaContainer = sirenaContainer;
    this.messageBuilderFactory = messageBuilderFactory;
  }
  protected override IMessageBuilder CreateNotification(IRequestContext context)
  {
    var sirena = sirenaContainer.Get();
    return messageBuilderFactory.Create(context, sirena);
  }
}