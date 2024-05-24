using Hedgey.Sirena.Database;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

/// <summary>
/// Provide subscribers with extra text or image or whatever
/// </summary>
public class AddExtraInformationStep : CommandStep
{
  private readonly NullableContainer<SirenRepresentation> sirenaContainer;
  private readonly NullableContainer<Message> messageContainer;
  private bool userNotified = false;
  public AddExtraInformationStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer, NullableContainer<Message> messageContainer) : base(contextContainer)
  {
    this.sirenaContainer = sirenaContainer;
    this.messageContainer = messageContainer;
  }

  public override IObservable<Report> Make()
  {
    var chatId = Context.GetTargetChatId();
    var sirena = sirenaContainer.Get();
    var message = Context.GetMessage();
    Report report;
    if (message.From.IsBot && !userNotified)
    {
      userNotified = true;
      report = new(Result.Wait, new ExtraInformationMessageBuilder(chatId, sirena));
    }
    else
    {
      if(!message.From.IsBot)
            messageContainer.Set(message);
      report = new(Result.Success);
    }
    return Observable.Return(report);
  }
}