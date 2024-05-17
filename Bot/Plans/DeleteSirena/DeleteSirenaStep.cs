using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;
public abstract class DeleteSirenaStep : CommandStep
{
  protected readonly NullableContainer<SirenRepresentation> sirenaContainer;

  public DeleteSirenaStep(Container<IRequestContext> contextContainer
  , NullableContainer<SirenRepresentation> sirenaContainer)
    : base(contextContainer)
  {
    this.sirenaContainer = sirenaContainer;
  }
}
