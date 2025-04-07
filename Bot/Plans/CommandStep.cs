using Hedgey.Structure.Plan;
using Hedgey.Telegram.Bot;
using System.Text;

namespace Hedgey.Sirena.Bot;

public abstract class CommandStep : IObservableStep<IRequestContext, CommandStep.Report>
{
  public abstract IObservable<Report> Make(IRequestContext context);
  public enum Result
  {
    Success,
    /// <summary>
    /// Plan can't be executed with current params
    /// and it waits
    /// </summary>
    Wait,
    /// <summary>
    /// Command can't be executed with current params
    /// and it stops
    /// </summary>
    Canceled,
    /// <summary>
    /// Unhandled exception of the code behaviour
    /// Connection and request exceptions
    /// </summary>
    Exception,
  }

  //TODO: Change IMessageBuilder<BaseRequest> -> IEnumerable<IMessageBuilder>
  public record class Report(Result Result, ISendMessageBuilder? MessageBuilder = null
  , IEditMessageBuilder? EditMessageBuilder = null
  , IEditMessageReplyMarkupBuilder? EditMessageReplyMarkupBuilder = null
  , IRequestContext? Fallback = null)
  {
    public Report(IRequestContext Fallback, ISendMessageBuilder? MessageBuilder = null
    , IEditMessageBuilder? EditMessageBuilder = null
    , IEditMessageReplyMarkupBuilder? EditMessageReplyMarkupBuilder = null)
    : this(Result.Canceled, MessageBuilder, EditMessageBuilder
      , EditMessageReplyMarkupBuilder, Fallback)
    { }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder("Report: [State: ")
      .Append(Result);

      if (MessageBuilder != null)
        builder.Append(" | ")
        .Append(MessageBuilder?.GetType().Name);

      if (EditMessageBuilder != null)
        builder.Append(" | ")
        .Append(EditMessageBuilder?.GetType().Name);

      builder.Append(']');
      return builder.ToString();
    }
  }
}