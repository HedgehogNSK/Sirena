using Hedgey.Sirena.Database;
using Hedgey.Structure.Factory;

namespace Hedgey.Sirena.Bot;

public class AddRequestMessageStep(NullableContainer<SirenRepresentation> sirenaContainer
  , IFactory<IRequestContext, SirenRepresentation, IMessageBuilder> messageBuilderFactory)
  : NotificationStep
{
  protected override IMessageBuilder CreateNotification(IRequestContext context)
  {
    var sirena = sirenaContainer.Get();
    return messageBuilderFactory.Create(context, sirena);
  }
  public class Factory(IFactory<IRequestContext, SirenRepresentation, IMessageBuilder> messageBuilderFactory)
  : IFactory<NullableContainer<SirenRepresentation>, AddRequestMessageStep>
  {
    public AddRequestMessageStep Create(NullableContainer<SirenRepresentation> sirenaContainer)
      => new AddRequestMessageStep(sirenaContainer, messageBuilderFactory);
  }
}