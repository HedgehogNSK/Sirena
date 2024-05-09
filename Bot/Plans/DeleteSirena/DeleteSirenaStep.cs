using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;
public abstract class DeleteSirenaStep : CommandStep
{
  protected Container<SirenRepresentation> sirenaContainer;
  public DeleteSirenaStep(Container<IRequestContext> contextContainer, Container<SirenRepresentation> sirenaContainer)
    : base(contextContainer)
    => this.sirenaContainer = sirenaContainer;

}
