using Hedgey.Localization;
using Hedgey.Structure.Factory;
using RxTelegram.Bot.Interface.BaseTypes;
using System.Reactive.Linq;

namespace Hedgey.Sirena.Bot;

/// <summary>
/// Provide subscribers with extra text or image or whatever
/// </summary>
public class AddExtraInformationStep : CommandStep
{
  private readonly NullableContainer<Message> messageContainer;
  private readonly IFactory<IRequestContext, IMessageBuilder> messageBuilderFactory;
  private bool userNotified = false;
  public AddExtraInformationStep(NullableContainer<Message> messageContainer
  , IFactory<IRequestContext, IMessageBuilder> messageBuilderFactory)
  {
    this.messageContainer = messageContainer;
    this.messageBuilderFactory = messageBuilderFactory;
  }

  public override IObservable<Report> Make(IRequestContext context)
  {
    var message = context.GetMessage();
    Report report;
    if (message.From.IsBot && !userNotified)
    {
      userNotified = true;
      var messageBuilder = messageBuilderFactory.Create(context);
      report = new(Result.Wait, messageBuilder);
    }
    else
    {
      if (!message.From.IsBot)
      {
        var commandName = context.GetCommandName();
        if (string.IsNullOrEmpty(commandName))
          messageContainer.Set(message);
      }
      report = new(Result.Success);
    }
    return Observable.Return(report);
  }

  public class Factory(IFactory<IRequestContext, IMessageBuilder> messageBuilderFactory)
    : IFactory<NullableContainer<Message>, AddExtraInformationStep>
  {
    public AddExtraInformationStep Create(NullableContainer<Message> messageContainer)
      => new AddExtraInformationStep(messageContainer, messageBuilderFactory);
  }

  public class MessagBuilderFactory(ILocalizationProvider localizationProvider)
    : IFactory<IRequestContext, IMessageBuilder>
  {
    public IMessageBuilder Create(IRequestContext context)
    {
      const string messageKey = "command.call.extra_info";
      const string skipButtonKey = messageKey + ".skip";
      return new OptionalDataRequireMessageBuilder(context, localizationProvider, messageKey, skipButtonKey);
    }
  }
}