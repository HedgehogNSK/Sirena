using Hedgey.Sirena.Database;

namespace Hedgey.Sirena.Bot.Operations
{
  public interface IDeleteSirenaOperation
  {
    IObservable<SirenRepresentation> Delete(long userId, ulong sirenaId);
  }
}