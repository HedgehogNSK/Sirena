using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;

public abstract class CreateSirenaStep : CommandStep
{
  protected Buffer buffer;
  public CreateSirenaStep(Container<IRequestContext> contextContainer, Buffer buffer)
    : base(contextContainer)
    => this.buffer = buffer;

  public class Buffer
  {
    public UserRepresentation User { get; set; }
    public CreateMessageBuilder MessageBuilder { get; set; }
    public string SirenaTitle { get; set; }

    public Buffer(CreateMessageBuilder messageBuilder)
    {
      MessageBuilder = messageBuilder;
    }
  }
}
