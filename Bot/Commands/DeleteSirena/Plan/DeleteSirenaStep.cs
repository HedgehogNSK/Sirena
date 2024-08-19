using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot;
public abstract class DeleteSirenaStep : CommandStep
{
  protected readonly NullableContainer<SirenRepresentation> sirenaContainer;

  public DeleteSirenaStep(NullableContainer<SirenRepresentation> sirenaContainer)
  {
    this.sirenaContainer = sirenaContainer;
  }
}