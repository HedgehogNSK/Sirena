using Hedgey.Sirena.Entities;

namespace Hedgey.Sirena.Bot;
public abstract class DeleteSirenaStep : CommandStep
{
  protected readonly NullableContainer<SirenaData> sirenaContainer;

  public DeleteSirenaStep(NullableContainer<SirenaData> sirenaContainer)
  {
    this.sirenaContainer = sirenaContainer;
  }
}