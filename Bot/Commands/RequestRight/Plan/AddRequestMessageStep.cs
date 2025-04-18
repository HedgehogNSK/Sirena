using Hedgey.Sirena.Entities;
using Hedgey.Structure.Factory;
using Hedgey.Telegram.Bot;

namespace Hedgey.Sirena.Bot;

public class AddRequestMessageStep(NullableContainer<SirenaData> sirenaContainer
  , IFactory<IRequestContext, SirenaData, ISendMessageBuilder> messageBuilderFactory)
  : NotificationStep
{
  protected override ISendMessageBuilder CreateNotification(IRequestContext context)
  {
    var sirena = sirenaContainer.Get();
    return messageBuilderFactory.Create(context, sirena);
  }
  public class Factory(IFactory<IRequestContext, SirenaData, ISendMessageBuilder> messageBuilderFactory)
  : IFactory<NullableContainer<SirenaData>, AddRequestMessageStep>
  {
    public AddRequestMessageStep Create(NullableContainer<SirenaData> sirenaContainer)
      => new AddRequestMessageStep(sirenaContainer, messageBuilderFactory);
  }
}