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
    public UserRepresentation? User { get; set; }=null;
    public CreateMessageBuilder MessageBuilder { get; set; }
    public string SirenaTitle { get; set; } = string.Empty;

    public Buffer(CreateMessageBuilder messageBuilder)
    {
      MessageBuilder = messageBuilder;
    }
    public UserRepresentation GetUser() => User ?? throw new ArgumentNullException(nameof(User));
  }
}
